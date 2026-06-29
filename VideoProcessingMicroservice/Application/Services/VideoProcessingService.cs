using System.Diagnostics;
using Shared.DTO;
using Shared.Enums;
using VideoProcessingMicroservice.Application.Abstractions;
using VideoProcessingMicroservice.Core;
using VideoProcessingMicroservice.Core.Abstractions;

namespace VideoProcessingMicroservice.Application.Services;

public class VideoProcessingService : IVideoProcessingService
{
    private readonly IVideoStorage _videoStorage;
    private readonly IVideoProcessor _videoProcessor;
    private readonly IVideoMetadataClient _videoMetadataClient;

    public VideoProcessingService(IVideoStorage videoStorage, IVideoProcessor videoProcessor, 
        IVideoMetadataClient videoMetadataClient)
    {
        _videoStorage = videoStorage;
        _videoProcessor = videoProcessor;
        _videoMetadataClient = videoMetadataClient;
    }
    
    public async Task ProcessVideoAsync(string key, Guid id, AdaptiveFormat format, CancellationToken cancellationToken)
    {
        var formatConfig = format switch
        {
            AdaptiveFormat.Hls => new FormatConfig
            {
                OutputDirName = "hls",
                ManifestFileName = "master.m3u8",
                S3StorageDirName = "hls"
            },
            AdaptiveFormat.Dash => new FormatConfig
            {
                OutputDirName = "dash",
                ManifestFileName = "manifest.mpd",
                S3StorageDirName = "dash"
            }
        };       

        var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var inputDir = Path.Combine(tempRoot, "input");
        var outputDir = Path.Combine(tempRoot, formatConfig.OutputDirName);

        Directory.CreateDirectory(inputDir);
        Directory.CreateDirectory(outputDir);
        
        var inputPath = Path.Combine(inputDir, Path.GetFileName(key));

        try
        {
            await using var inputStream = await _videoStorage.DownloadAsync(key, cancellationToken);
            
            var sw = Stopwatch.StartNew();
            
            await _videoProcessor.ProcessAsync(inputStream, inputPath, outputDir, format, cancellationToken);
            
            sw.Stop();

            Console.WriteLine($"Время обработки: {sw.Elapsed}");

            var videoId = Path.GetFileNameWithoutExtension(key);
            string s3Key;
            
            foreach (var file in Directory.GetFiles(outputDir))
            {
                await using var fs = File.OpenRead(file);
                s3Key = $"{formatConfig.S3StorageDirName}/{videoId}/{Path.GetFileName(file)}";
                await _videoStorage.UploadAsync(fs, s3Key, cancellationToken);
            }
            
            s3Key = $"{formatConfig.S3StorageDirName}/{videoId}/{formatConfig.ManifestFileName}";
            
            var setKeyTask = _videoMetadataClient.CreateManifestAsync(new CreateVideoManifestRequest(
                id,
                format == AdaptiveFormat.Hls ? "hls" : "dash",
                s3Key), cancellationToken);
            var updateStatusTask = _videoMetadataClient.UpdateStatusAsync(new UpdateVideoStatusRequest(
                id,
                VideoStatus.Ready), cancellationToken);

            await Task.WhenAll(setKeyTask, updateStatusTask);
        }
        catch
        {
            await _videoMetadataClient.UpdateStatusAsync(new UpdateVideoStatusRequest(
                id,
                VideoStatus.Failed), cancellationToken);
        }
        finally
        {
            if (Directory.Exists(tempRoot))
                Directory.Delete(tempRoot, true);
        }
    }
    
    public async Task ConvertVideoAsync(string key, Guid id, CancellationToken cancellationToken)
    {
        await using var inputStream = await _videoStorage.DownloadAsync(key, cancellationToken);
        inputStream.Position = 0;

        var resolutions = new Dictionary<string, string>
        {
            { "1080p", "1920x1080" },
            { "720p", "1280x720" },
            { "480p", "854x480" },
            { "360p", "640x360" },
            { "240p", "426x240" },
            { "144p", "256x144" }
        };

        var videoId = Path.GetFileNameWithoutExtension(key);
        var extension = Path.GetExtension(key);

        foreach (var res in resolutions)
        {
            var outputStream = new MemoryStream();

            await _videoProcessor.ConvertToSpecificResolutionAsync(inputStream, outputStream, res.Value,
                cancellationToken);

            outputStream.Position = 0;
            var s3Key = $"full/{videoId}/{res.Key}{extension}";
            
            await _videoStorage.UploadAsync(outputStream, s3Key, cancellationToken);
            await outputStream.DisposeAsync();
            
            inputStream.Position = 0; // сброс для следующей конверсии
        }
        var setKeyTask = _videoMetadataClient.CreateManifestAsync(new CreateVideoManifestRequest(
            id,
            "None",
            $"full/{videoId}"), cancellationToken);
        var updateStatusTask = _videoMetadataClient.UpdateStatusAsync(new UpdateVideoStatusRequest(
            id,
            VideoStatus.Ready), cancellationToken);

        await Task.WhenAll(setKeyTask, updateStatusTask);
    }
}