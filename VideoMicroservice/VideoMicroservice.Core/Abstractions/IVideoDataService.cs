using VideoMicroservice.VideoMicroservice.Core.Models;
using VideoMicroservice.VideoMicroservice.Core.Models.Results;

namespace VideoMicroservice.VideoMicroservice.Core.Abstractions;

public interface IVideoDataService
{
    Task<Result<Guid>> CreateVideoDataAsync(string title, string description,
        string url, string thumbnailUrl);

    Task<Result<VideoData>> GetVideoDataAsync(Guid id);
    Task<Result<List<VideoData>>> GetAllVideoDataAsync();

    Task<Result<Guid>> UpdateVideoDataAsync(Guid id, string title, string description, 
        string thumbnailUrl);

    Task<Result<Guid>> DeleteVideoDataAsync(Guid id);
}