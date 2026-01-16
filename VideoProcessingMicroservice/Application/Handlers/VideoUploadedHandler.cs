using VideoProcessingMicroservice.Application.Abstractions;
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
        await _videoProcessingService.ProcessAsync(key, id, cancellationToken);
    }
}
