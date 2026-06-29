using System.Diagnostics;
using VideoProcessingMicroservice.Application.Abstractions;
using VideoProcessingMicroservice.Core;

namespace VideoProcessingMicroservice.Infrastructure.Ffmpeg;

public class FfmpegVideoProcessor : IVideoProcessor
{
    private readonly string _ffmpegPath;

    public FfmpegVideoProcessor(IConfiguration config)
    {
        _ffmpegPath = config["FFmpeg:Path"]!;
    }
    
    public async Task ProcessAsync(
        Stream input,
        string inputPath,
        string outputDir,
        AdaptiveFormat format,
        CancellationToken cancellationToken)
    {
        await using (var fs = File.Create(inputPath))
            await input.CopyToAsync(fs, cancellationToken);

        string args = format switch
        {
            AdaptiveFormat.Hls => $@"
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
                ".Replace("\r", "").Replace("\n", " "),
            AdaptiveFormat.Dash => $@"
                -hide_banner -y
                -i ""{inputPath}""
                -filter_complex ""[0:v]split=3[v0][v1][v2];
                [v0]scale=-2:360,setsar=1,setdar=16/9[v0out];
                [v1]scale=-2:480,setsar=1,setdar=16/9[v1out];
                [v2]scale=-2:720,setsar=1,setdar=16/9[v2out]""
                -map ""[v0out]""
                -map ""[v1out]""
                -map ""[v2out]""
                -map 0:a:0
                -c:v libx264
                -profile:v high
                -b:v:0 600k -maxrate:v:0 700k -bufsize:v:0 1200k
                -b:v:1 1000k -maxrate:v:1 1100k -bufsize:v:1 2000k
                -b:v:2 2500k -maxrate:v:2 2700k -bufsize:v:2 5000k
                -c:a aac
                -b:a 128k
                -ac 2
                -g 48 -keyint_min 48 -sc_threshold 0
                -seg_duration 4
                -use_template 1
                -use_timeline 1
                -init_seg_name ""init_$RepresentationID$.$ext$""
                -media_seg_name ""chunk_$RepresentationID$_$Number%05d$.$ext$""
                -adaptation_sets ""id=0,streams=0,1,2 id=1,streams=3""
                -f dash
                ""{outputDir}/manifest.mpd""
                ".Replace("\r", "").Replace("\n", " ")
            
        };
        
        /*
         AdaptiveFormat.Dash => $@"
                -hide_banner -y
                -i ""{inputPath}""
                -filter_complex ""[0:v]split=3[v0][v1][v2];
                [v0]scale=-2:360,setsar=1,setdar=16/9[v0out];
                [v1]scale=-2:480,setsar=1,setdar=16/9[v1out];
                [v2]scale=-2:720,setsar=1,setdar=16/9[v2out]""
                -map ""[v0out]"" -map 0:a -c:v:0 libx264 -profile:v:0 high -b:v:0 800k -g 48 -keyint_min 48
                -map ""[v1out]"" -map 0:a -c:v:1 libx264 -profile:v:1 high -b:v:1 1500k -g 48 -keyint_min 48
                -map ""[v2out]"" -map 0:a -c:v:2 libx264 -profile:v:2 high -b:v:2 3000k -g 48 -keyint_min 48
                -c:a aac -b:a 128k -ac 2
                -f dash
                -window_size 0
                -extra_window_size 0
                -remove_at_exit 0
                -seg_duration 4
                -use_template 1
                -use_timeline 1
                -init_seg_name ""init_$RepresentationID$.$ext$""
                -media_seg_name ""chunk_$RepresentationID$_$Number%05d$.$ext$""
                -adaptation_sets ""id=0,streams=v id=1,streams=a""
                ""{outputDir}/manifest.mpd""
            ".Replace("\r", "").Replace("\n", " ")
         */
        
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