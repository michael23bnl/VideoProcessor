using Shared.Enums;

namespace Shared.DTO;

public record UpdateVideoStatusRequest(Guid Id, VideoStatus Status);