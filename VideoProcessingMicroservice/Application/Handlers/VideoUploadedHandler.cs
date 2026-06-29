using VideoProcessingMicroservice.Application.Abstractions;
using VideoProcessingMicroservice.Core;
using VideoProcessingMicroservice.Core.Abstractions;

namespace VideoProcessingMicroservice.Application.Handlers;

public class VideoUploadedHandler : IVideoUploadedHandler
{
    private readonly IVideoProcessingService _videoProcessingService;

    public VideoUploadedHandler(IVideoProcessingService videoProcessingService)
    {
        _videoProcessingService = videoProcessingService;
    }

    public async Task HandleAsync(string key, Guid id, CancellationToken cancellationToken)
    {
        // var hlsProcessingTask = _videoProcessingService
        //     .ProcessVideoAsync(key, id, AdaptiveFormat.Hls, cancellationToken);
        // var dashProcessingTask = _videoProcessingService
        //     .ProcessVideoAsync(key, id, AdaptiveFormat.Dash, cancellationToken);
        //
        // await Task.WhenAll(hlsProcessingTask, dashProcessingTask);
        
        //await _videoProcessingService.ProcessVideoAsync(key, id, AdaptiveFormat.Hls, cancellationToken);
        await _videoProcessingService.ProcessVideoAsync(key, id, AdaptiveFormat.Dash, cancellationToken);

        
    }
}
