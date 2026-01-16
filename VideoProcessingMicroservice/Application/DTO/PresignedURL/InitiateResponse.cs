namespace VideoProcessingMicroservice.Application.DTO.PresignedURL;

public record InitiateResponse(string UploadId, string Key, List<PresignedPartUrl> Parts, Guid VideoId);