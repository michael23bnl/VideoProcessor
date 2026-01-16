namespace VideoProcessingMicroservice.Application.DTO.PresignedURL;

public record CompleteUploadRequest(string UploadId, string Key, List<CompletedPart> Parts, Guid VideoId);