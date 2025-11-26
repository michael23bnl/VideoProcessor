namespace FileProcessingMicroservice.DTO.PresignedURL;

public record UploadRequest(string FileName, long FileSize);