using System.Diagnostics;
using VideoProcessingMicroservice.Application.Abstractions;
using VideoProcessingMicroservice.Core;

namespace VideoProcessingMicroservice.Infrastructure.Ffmpeg;

public class FfmpegVideoMultiProcessor : IVideoProcessor
{
    private readonly string _ffmpegPath;

    public FfmpegVideoMultiProcessor(IConfiguration config)
    {
        _ffmpegPath = config["FFmpeg:Path"]!;
    }

    private record VideoProfile(
        string Name,
        int Height,
        string Bitrate);

    private static readonly VideoProfile[] Profiles =
    [
        new("360p", 360, "800k"),
        new("480p", 480, "1500k"),
        new("720p", 720, "3000k")
    ];

    public async Task ProcessAsync(
        Stream input,
        string inputPath,
        string outputDir,
        AdaptiveFormat format,
        CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(outputDir);

        await using (var fs = File.Create(inputPath))
        {
            await input.CopyToAsync(fs, cancellationToken);
        }

        switch (format)
        {
            case AdaptiveFormat.Hls:
            {
                var hlsTasks = Profiles.Select(profile =>
                    RunFfmpegAsync(
                        BuildHlsArguments(
                            inputPath,
                            outputDir,
                            profile),
                        outputDir,
                        cancellationToken));

                await Task.WhenAll(hlsTasks);
                await CreateMasterPlaylistAsync(outputDir);

                break;
            }

            case AdaptiveFormat.Dash:
            {
                var dashTasks = Profiles.Select(profile =>
                    RunFfmpegAsync(
                        BuildDashArguments(
                            inputPath,
                            outputDir,
                            profile),
                        outputDir,
                        cancellationToken));

                await Task.WhenAll(dashTasks);

                try
                {
                    await CreateDashManifestAsync(outputDir, cancellationToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                

                break;
            }

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(format),
                    format,
                    null);
        }
    }

    private async Task RunFfmpegAsync(
        string args,
        string workingDir,
        CancellationToken cancellationToken)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = _ffmpegPath,
            Arguments = args,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            
            WorkingDirectory = workingDir
        };

        using var process = Process.Start(startInfo);
        var stderr = await process.StandardError.ReadToEndAsync(cancellationToken);

        await process.WaitForExitAsync(cancellationToken);

        if (process.ExitCode != 0)
        {
            throw new Exception(
                $"FFmpeg завершился с ошибкой:\n{stderr}");
        }
    }

    private string BuildHlsArguments(
        string inputPath,
        string outputDir,
        VideoProfile profile)
    {
        return $@"
            -hide_banner -y
            -i ""{inputPath}""
            -vf scale=-2:{profile.Height}
            -c:v libx264
            -profile:v high
            -b:v {profile.Bitrate}
            -g 48
            -c:a aac
            -b:a 128k
            -ac 2
            -f hls
            -hls_time 4
            -hls_list_size 0
            -hls_segment_filename ""{Path.Combine(outputDir, profile.Name)}_%06d.ts""
            ""{Path.Combine(outputDir, $"{profile.Name}.m3u8")}""
        "
        .Replace("\r", "")
        .Replace("\n", " ");
    }

    private string BuildDashArguments(
        string inputPath,
        string outputDir,
        VideoProfile profile)
    {
        return $@"
            -hide_banner -y
            -i ""{inputPath}""
            -vf scale=-2:{profile.Height},setsar=1,setdar=16/9
            -c:v libx264
            -profile:v high
            -b:v {profile.Bitrate}
            -g 48
            -keyint_min 48
            -c:a aac
            -b:a 128k
            -ac 2
            -movflags +faststart
            ""{Path.Combine(outputDir, $"{profile.Name}.mp4")}""
        "
        .Replace("\r", "")
        .Replace("\n", " ");
    }

    private async Task CreateMasterPlaylistAsync(
        string outputDir)
    {
        var masterPlaylist = """
            #EXTM3U

            #EXT-X-STREAM-INF:BANDWIDTH=800000,RESOLUTION=640x360
            360p.m3u8

            #EXT-X-STREAM-INF:BANDWIDTH=1500000,RESOLUTION=854x480
            480p.m3u8

            #EXT-X-STREAM-INF:BANDWIDTH=3000000,RESOLUTION=1280x720
            720p.m3u8
            """;

        await File.WriteAllTextAsync(
            Path.Combine(outputDir, "master.m3u8"),
            masterPlaylist);
    }

    private async Task CreateDashManifestAsync(
        string outputDir,
        CancellationToken cancellationToken)
    {
        var args = $@"
            -hide_banner -y

            -i ""{Path.Combine(outputDir, "360p.mp4")}""
            -i ""{Path.Combine(outputDir, "480p.mp4")}""
            -i ""{Path.Combine(outputDir, "720p.mp4")}""

            -map 0:v:0
            -map 1:v:0
            -map 2:v:0

            -map 0:a:0

            -c copy

            -f dash

            -window_size 0
            -extra_window_size 0
            -remove_at_exit 0

            -seg_duration 4

            -use_template 1
            -use_timeline 1

            -init_seg_name ""init_$RepresentationID$.m4s""
            -media_seg_name ""chunk_$RepresentationID$_$Number%05d$.m4s""

            -adaptation_sets ""id=0,streams=0,1,2 id=1,streams=3""

            ""{Path.Combine(outputDir, "manifest.mpd")}""
        "
        .Replace("\r", "")
        .Replace("\n", " ");

        await RunFfmpegAsync(
            args,
            outputDir,
            cancellationToken);

        foreach (var profile in Profiles)
        {
            var file =
                Path.Combine(
                    outputDir,
                    $"{profile.Name}.mp4");

            if (File.Exists(file))
                File.Delete(file);
        }
    }

    public async Task ConvertToSpecificResolutionAsync(
        Stream input,
        Stream output,
        string resolution,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}