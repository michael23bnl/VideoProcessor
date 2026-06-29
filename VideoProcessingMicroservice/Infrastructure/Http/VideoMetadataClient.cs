using Shared.DTO;
using VideoProcessingMicroservice.Application.Abstractions;

namespace VideoProcessingMicroservice.Infrastructure.Http;

public class VideoMetadataClient : IVideoMetadataClient
{
    private readonly HttpClient _httpClient;

    public VideoMetadataClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task CreateMetadataAsync(CreateVideoMetadataRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync("/videodata/metadata", request, cancellationToken);
        
        response.EnsureSuccessStatusCode();
    }
    
    public async Task CreateManifestAsync(CreateVideoManifestRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync("/videodata/manifest", request, cancellationToken);
        
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateStatusAsync(UpdateVideoStatusRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PutAsJsonAsync("/videodata/metadata/status", request, cancellationToken);

        response.EnsureSuccessStatusCode();
    }
}