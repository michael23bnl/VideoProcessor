using VideoProcessingMicroservice.Application.DTO;

namespace VideoProcessingMicroservice.Application.Abstractions;

public interface IMessageProducer
{
    Task SendMessageAsync(VideoUploadedEvent message, CancellationToken ct);
}