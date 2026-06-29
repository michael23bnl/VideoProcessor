namespace VideoDataMicroservice.Application.DTO;

public record UploadedVideoData(List<VideoManifestInfo> Manifests, string Title, DateTime UploadedAt);