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

    public async Task CreateAsync(CreateVideoMetadataRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync("/videodata/create", request, cancellationToken);
        
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateStatusAsync(UpdateVideoStatusRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PutAsJsonAsync("/videodata/update-status", request, cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public async Task SetKeyAsync(SetVideoKeyRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PutAsJsonAsync("/videodata/set-key", request, cancellationToken);
        
        response.EnsureSuccessStatusCode();
    }
}