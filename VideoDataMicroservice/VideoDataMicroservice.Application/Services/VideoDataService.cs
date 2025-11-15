using VideoDataMicroservice.VideoDataMicroservice.Core.Abstractions;
using VideoDataMicroservice.VideoDataMicroservice.Core.Models;
using VideoDataMicroservice.VideoDataMicroservice.Core.Models.Errors;
using VideoDataMicroservice.VideoDataMicroservice.Core.Models.Results;

namespace VideoDataMicroservice.VideoDataMicroservice.Application.Services;

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
            return Result<Guid>.Failure(VideoDataError.InvalidTitle);
        
        if (string.IsNullOrEmpty(url))
            return Result<Guid>.Failure(VideoDataError.InvalidUrl);

        if (string.IsNullOrEmpty(thumbnailUrl))
            return Result<Guid>.Failure(VideoDataError.InvalidThumbnailUrl);

        var videoDataId = await _videoDataRepository.CreateAsync(title, description, url, thumbnailUrl);
    
        return Result<Guid>.Success(videoDataId);
    }

    public async Task<Result<VideoData>> GetVideoDataAsync(Guid id)
    {
        var videoData = await _videoDataRepository.GetAsync(id);

        if (videoData is null)
        {
            return Result<VideoData>.Failure(VideoDataError.NotFound);
        }
        
        return Result<VideoData>.Success(videoData);
    }

    public async Task<Result<List<VideoData>>> GetAllVideoDataAsync()
    {
        var videoData = await _videoDataRepository.GetAllAsync();

        return Result<List<VideoData>>.Success(videoData);
    }

    public async Task<Result<Guid>> UpdateVideoDataAsync(Guid id, string title, string description, 
        string thumbnailUrl)
    {
        var rowsUpdated = await _videoDataRepository.UpdateAsync(id, title, description, thumbnailUrl);

        if (rowsUpdated == 0)
        {
            return Result<Guid>.Failure(VideoDataError.NotFound);
        }
        
        return Result<Guid>.Success(id);
    }

    public async Task<Result<Guid>> DeleteVideoDataAsync(Guid id)
    {
        var rowsDeleted = await _videoDataRepository.DeleteAsync(id);

        if (rowsDeleted == 0)
        {
            return Result<Guid>.Failure(VideoDataError.NotFound);
        }

        return Result<Guid>.Success(id);
    }
    
}