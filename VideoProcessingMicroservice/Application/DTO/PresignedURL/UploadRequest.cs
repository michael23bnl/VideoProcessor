namespace VideoProcessingMicroservice.Application.DTO.PresignedURL;

public record UploadRequest(string FileName, long FileSize);