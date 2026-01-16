using Shared.Enums;

namespace VideoDataMicroservice.Core.Models;

public class VideoData
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public DateTime? UploadedAt { get; set; }
    public VideoStatus Status { get; set; }
    public string? Key { get; set; }
    public string? ThumbnailUrl { get; set; }
}