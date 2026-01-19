using Shared.DTO;
using Shared.Enums;
using VideoProcessingMicroservice.Application.Abstractions;
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

    public async Task ProcessAsync(string key, Guid id, CancellationToken cancellationToken)
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var inputDir = Path.Combine(tempRoot, "input");
        var outputDir = Path.Combine(tempRoot, "hls");

        Directory.CreateDirectory(inputDir);
        Directory.CreateDirectory(outputDir);
        
        var inputPath = Path.Combine(inputDir, Path.GetFileName(key));

        try
        {
            await using var inputStream = await _videoStorage.DownloadAsync(key, cancellationToken);
            await _videoProcessor.ProcessToHlsAsync(inputStream, inputPath, outputDir, cancellationToken);

            var videoId = Path.GetFileNameWithoutExtension(key);
            string s3Key;
            
            foreach (var file in Directory.GetFiles(outputDir))
            {
                await using var fs = File.OpenRead(file);
                s3Key = $"processed/{videoId}/{Path.GetFileName(file)}";
                await _videoStorage.UploadAsync(fs, s3Key, cancellationToken);
            }
            
            s3Key = $"processed/{videoId}/master.m3u8";
            
            var setKeyTask = _videoMetadataClient.SetKeyAsync(new SetVideoKeyRequest(
                id,
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

    
    public async Task ConvertAsync(string key, Guid id, CancellationToken cancellationToken)
    {
        // загружаем исходное видео в поток
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

            // создаем потоковую конвертацию через FFmpeg
            await _videoProcessor.ConvertToSpecificResolutionAsync(inputStream, outputStream, res.Value,
                cancellationToken);

            // загружаем результат обратно в S3
            outputStream.Position = 0;
            var s3Key = $"converted/{videoId}/{res.Key}{extension}";
            
            await _videoStorage.UploadAsync(outputStream, s3Key, cancellationToken);
            await outputStream.DisposeAsync();
            
            inputStream.Position = 0; // сброс для следующей конверсии
        }
        var setKeyTask = _videoMetadataClient.SetKeyAsync(new SetVideoKeyRequest(
            id,
            $"converted/{videoId}"), cancellationToken);
        var updateStatusTask = _videoMetadataClient.UpdateStatusAsync(new UpdateVideoStatusRequest(
            id,
            VideoStatus.Ready), cancellationToken);

        await Task.WhenAll(setKeyTask, updateStatusTask);
    }
}