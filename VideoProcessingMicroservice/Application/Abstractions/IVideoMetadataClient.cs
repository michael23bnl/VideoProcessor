using Shared.DTO;
using Shared.Enums;

namespace VideoProcessingMicroservice.Application.Abstractions;

public interface IVideoMetadataClient
{
    Task CreateMetadataAsync(CreateVideoMetadataRequest request, CancellationToken cancellationToken);
    Task UpdateStatusAsync(UpdateVideoStatusRequest request, CancellationToken cancellationToken);
    Task CreateManifestAsync(CreateVideoManifestRequest request, CancellationToken cancellationToken);
}