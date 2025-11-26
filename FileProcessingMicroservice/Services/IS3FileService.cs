using FileProcessingMicroservice.DTO.PresignedURL;

namespace FileProcessingMicroservice.Services;

public interface IS3FileService
{
    Task<InitiateResponse> InitiateUploadAsync(UploadRequest request);
    Task<UploadCompletedResponse> CompleteUploadAsync(CompleteUploadRequest request);
    Task<string> GetDownloadUrlAsync(string key);
}