using Shared.DTO;
using Shared.Enums;

namespace VideoProcessingMicroservice.Application.Abstractions;

public interface IVideoMetadataClient
{
    Task CreateAsync(CreateVideoMetadataRequest request, CancellationToken cancellationToken);
    Task UpdateStatusAsync(UpdateVideoStatusRequest request, CancellationToken cancellationToken);
    Task SetKeyAsync(SetVideoKeyRequest request, CancellationToken cancellationToken);
}