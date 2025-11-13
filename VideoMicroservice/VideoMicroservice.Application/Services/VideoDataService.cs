using VideoMicroservice.VideoMicroservice.Core.Abstractions;
using VideoMicroservice.VideoMicroservice.Core.Models;
using VideoMicroservice.VideoMicroservice.Core.Models.Errors;
using VideoMicroservice.VideoMicroservice.Core.Models.Results;

namespace VideoMicroservice.VideoMicroservice.Application.Services;

public class VideoDataService : IVideoDataService
{
    private readonly IVideoDataRepository _videoDataRepository;
    
    public VideoDataService(IVideoDataRepository videoDataRepository)
    {
        _videoDataRepository = videoDataRepository;
    }

    public async Task<Result<Guid>> CreateVideoDataAsync(
        string title,
        string description,
        string url,
        string thumbnailUrl)
    {
        if (string.IsNullOrEmpty(title))
            return Result.Failure<Guid>(VideoDataError.InvalidTitle);
        
        if (string.IsNullOrEmpty(url))
            return Result.Failure<Guid>(VideoDataError.InvalidUrl);

        if (string.IsNullOrEmpty(thumbnailUrl))
            return Result.Failure<Guid>(VideoDataError.InvalidThumbnailUrl);

        var videoDataId = await _videoDataRepository.CreateAsync(title, description, url, thumbnailUrl);
    
        return Result.Success(videoDataId);
    }

    public async Task<Result<VideoData>> GetVideoDataAsync(Guid id)
    {
        var videoData = await _videoDataRepository.GetAsync(id);

        if (videoData is null)
        {
            return Result.Failure<VideoData>(VideoDataError.NotFound);
        }
        
        return Result.Success(videoData);
    }

    public async Task<Result<List<VideoData>>> GetAllVideoDataAsync()
    {
        var videoData = await _videoDataRepository.GetAllAsync();

        return Result.Success(videoData);
    }

    public async Task<Result<Guid>> UpdateVideoDataAsync(Guid id, string title, string description, 
        string thumbnailUrl)
    {
        var rowsUpdated = await _videoDataRepository.UpdateAsync(id, title, description, thumbnailUrl);

        if (rowsUpdated == 0)
        {
            return Result.Failure<Guid>(VideoDataError.NotFound);
        }
        
        return Result.Success(id);
    }

    public async Task<Result<Guid>> DeleteVideoDataAsync(Guid id)
    {
        var rowsDeleted = await _videoDataRepository.DeleteAsync(id);

        if (rowsDeleted == 0)
        {
            return Result.Failure<Guid>(VideoDataError.NotFound);
        }

        return Result.Success(id);
    }
    
}