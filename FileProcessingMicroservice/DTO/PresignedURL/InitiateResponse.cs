namespace FileProcessingMicroservice.DTO.PresignedURL;

public record InitiateResponse(string UploadId, string Key, List<PresignedPartUrl> Parts);