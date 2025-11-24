using FileProcessingMicroservice.DTO;

namespace FileProcessingMicroservice.Services;

public interface IFileService
{
    Task<FileUploadSummary> UploadFileAsync(Stream fileStream, string contentType);
}