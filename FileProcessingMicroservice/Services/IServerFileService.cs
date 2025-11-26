using FileProcessingMicroservice.DTO;

namespace FileProcessingMicroservice.Services;

public interface IServerFileService
{
    Task<FileUploadSummary> UploadFileAsync(Stream fileStream, string contentType);
}