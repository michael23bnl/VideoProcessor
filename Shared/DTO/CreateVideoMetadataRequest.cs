using Shared.Enums;

namespace Shared.DTO;

public record CreateVideoMetadataRequest(Guid Id, string FileName, VideoStatus Status);

