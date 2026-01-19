using System.Diagnostics;
using VideoProcessingMicroservice.Application.Abstractions;

namespace VideoProcessingMicroservice.Infrastructure.Ffmpeg;

public class FfmpegVideoProcessor : IVideoProcessor
{
    private readonly string _ffmpegPath;

    public FfmpegVideoProcessor(IConfiguration config)
    {
        _ffmpegPath = config["FFmpeg:Path"];
    }

    public async Task ProcessToHlsAsync(
        Stream input,
        string inputPath,
        string outputDir,
        CancellationToken cancellationToken)
    {
        await using (var fs = File.Create(inputPath))
            await input.CopyToAsync(fs, cancellationToken);

        var args = $@"
                -hide_banner -y 
                -i ""{inputPath}""
                -filter_complex ""[0:v]split=3[v0][v1][v2];
                [v0]scale=w=-2:h=360[v0out];
                [v1]scale=w=-2:h=480[v1out];
                [v2]scale=w=-2:h=720[v2out]""
                -map ""[v0out]"" -map 0:a -c:v:0 libx264 -profile:v:0 high -b:v:0 800k -g 48
                -map ""[v1out]"" -map 0:a -c:v:1 libx264 -profile:v:1 high -b:v:1 1500k -g 48
                -map ""[v2out]"" -map 0:a -c:v:2 libx264 -profile:v:2 high -b:v:2 3000k -g 48
                -c:a aac -b:a 128k -ac 2
                -f hls
                -var_stream_map ""v:0,a:0,name:360p v:1,a:1,name:480p v:2,a:2,name:720p""
                -hls_time 4
                -hls_list_size 0
                -hls_segment_filename ""{outputDir}\%v_%06d.ts""
                -master_pl_name master.m3u8
                ""{outputDir}\%v.m3u8""
                ".Replace("\r", "").Replace("\n", " ");

        var startInfo = new ProcessStartInfo
        {
            FileName = _ffmpegPath,
            Arguments = args,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo)!;

        var stderr = await process.StandardError.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);
        
        if (process.ExitCode != 0)
            throw new Exception($"FFmpeg error:\n{stderr}");
    }

    public async Task ConvertToSpecificResolutionAsync(
        Stream input,
        Stream output,
        string resolution,
        CancellationToken cancellationToken)
    {
        if (!File.Exists(_ffmpegPath))
            throw new FileNotFoundException("ffmpeg.exe не найден", _ffmpegPath);

        var args = $@"
            -y 
            -i pipe:0 
            -vf scale={resolution}
            -c:v libx264 
            -preset fast 
            -c:a aac 
            -movflags frag_keyframe+empty_moov 
            -f mp4 
            pipe:1".Replace("\r", "").Replace("\n", " ");

        var startInfo = new ProcessStartInfo
        {
            FileName = _ffmpegPath,
            Arguments = args,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo);

        var stderr = process.StandardError.ReadToEndAsync(cancellationToken);

        // копируем поток видео в stdin
        var inputTask = Task.Run(async () =>
        {
            await input.CopyToAsync(process.StandardInput.BaseStream, cancellationToken);
            process.StandardInput.Close(); // закрываем stdin, чтобы ffmpeg понял EOF
        }, cancellationToken);

        // копируем stdout ffmpeg в выходной поток
        var outputTask = process.StandardOutput.BaseStream.CopyToAsync(output, cancellationToken);

        await Task.WhenAll(inputTask, outputTask);

        await process.WaitForExitAsync(cancellationToken);
    }
}