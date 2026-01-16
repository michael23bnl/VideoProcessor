namespace VideoProcessingMicroservice.Application.Abstractions;

public interface IVideoUploadedHandler
{
    Task HandleAsync(string key, Guid id, CancellationToken cancellationToken);
}