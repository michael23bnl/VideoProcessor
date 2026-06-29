namespace VideoDataMicroservice.Core.Models;

public class VideoManifest
{
    public Guid Id { get; set; }
    public Guid VideoId { get; set; }
    public string Protocol { get; set; }
    public string S3Key { get; set; }
    public VideoMetadata Metadata { get; set; }
}