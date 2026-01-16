namespace VideoProcessingMicroservice.Core.Abstractions;

public interface IVideoProcessingService
{
    Task ProcessAsync(string key, Guid id, CancellationToken cancellationToken);
    Task ConvertAsync(string key, Guid id, CancellationToken cancellationToken);
}