using VideoProcessingMicroservice.Application.DTO.PresignedURL;

namespace VideoProcessingMicroservice.Core.Abstractions;

public interface IVideoUploadService
{
    Task<InitiateResponse> InitiateUploadAsync(UploadRequest request, CancellationToken cancellationToken);
    Task<UploadCompletedResponse> CompleteUploadAsync(CompleteUploadRequest request, CancellationToken cancellationToken);
    Task<string> GetDownloadUrlAsync(string key);
}