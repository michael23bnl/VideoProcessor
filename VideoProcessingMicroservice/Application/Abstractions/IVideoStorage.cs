using VideoProcessingMicroservice.Application.DTO.PresignedURL;

namespace VideoProcessingMicroservice.Application.Abstractions;

public interface IVideoStorage
{
    Task<InitiateResponse> InitiateUploadAsync(UploadRequest request, Guid videoId, CancellationToken cancellationToken);
    Task<UploadCompletedResponse> CompleteUploadAsync(CompleteUploadRequest request, CancellationToken cancellationToken);
    Task<string> GetDownloadUrlAsync(string key);
    public Task<Stream> DownloadAsync(string key, CancellationToken cancellationToken);
    public Task<long> UploadAsync(Stream fileStream, string key, CancellationToken cancellationToken);
}