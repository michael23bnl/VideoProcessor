namespace FileProcessingMicroservice.DTO.PresignedURL;

public record CompleteUploadRequest(string UploadId, string Key, List<CompletedPart> Parts);