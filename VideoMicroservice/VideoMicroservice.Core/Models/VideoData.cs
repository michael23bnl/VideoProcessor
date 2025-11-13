namespace VideoMicroservice.VideoMicroservice.Core.Models;

public class VideoData
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Url { get; set; }
    public string ThumbnailUrl { get; set; }
}