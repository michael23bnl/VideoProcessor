namespace VideoProcessingMicroservice.Core.Abstractions;

public interface IVideoProcessingService
{
    Task ProcessVideoAsync(string key, Guid id, AdaptiveFormat format, CancellationToken cancellationToken);
    Task ConvertVideoAsync(string key, Guid id, CancellationToken cancellationToken);
}