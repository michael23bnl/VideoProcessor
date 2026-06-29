namespace Shared.DTO;

public record CreateVideoManifestRequest(Guid Id, string Protocol, string S3Key);