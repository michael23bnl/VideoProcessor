using Shared.Enums;
using Shared.Result;
using VideoDataMicroservice.Application.DTO;
using VideoDataMicroservice.Core.Abstractions;
using VideoDataMicroservice.Core.Models;
using VideoDataMicroservice.Core.Models.Errors;

namespace VideoDataMicroservice.Application.Services;

public class VideoDataService : IVideoDataService
{
    private readonly IVideoDataRepository _videoDataRepository;
    
    public VideoDataService(IVideoDataRepository videoDataRepository)
    {
        _videoDataRepository = videoDataRepository;
    }

    public async Task<Result<Guid>> CreateVideoDataAsync(
        Guid id,
        string title,
        string? description,
        VideoStatus status,
        string? key,
        string? thumbnailUrl, 
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(title))
            return Result<Guid>.Failure(VideoDataError.InvalidTitle);
        
        /*if (string.IsNullOrEmpty(key))
            return Result<Guid>.Failure(VideoDataError.InvalidUrl);*/

        /*if (string.IsNullOrEmpty(thumbnailUrl))
            return Result<Guid>.Failure(VideoDataError.InvalidThumbnailUrl);*/

        var videoDataId = await _videoDataRepository.CreateAsync(id, title, description, status, key, thumbnailUrl, cancellationToken);
    
        return Result<Guid>.Success(videoDataId);
    }

    public async Task<Result<VideoData>> GetVideoDataAsync(Guid id, CancellationToken cancellationToken)
    {
        var videoData = await _videoDataRepository.GetAsync(id, cancellationToken);

        if (videoData is null)
        {
            return Result<VideoData>.Failure(VideoDataError.NotFound);
        }
        
        return Result<VideoData>.Success(videoData);
    }

    public async Task<Result<List<VideoData>>> GetAllVideoDataAsync(CancellationToken cancellationToken)
    {
        var videoData = await _videoDataRepository.GetAllFilteredAsync(null, cancellationToken);

        return Result<List<VideoData>>.Success(videoData);
    }

    public async Task<Result<List<UploadedVideoData>>> GetUploadedVideoDataAsync(CancellationToken cancellationToken)
    {
        var videoData = await _videoDataRepository.GetAllFilteredAsync(
            x => x.Status == VideoStatus.Ready, cancellationToken);
        
        var uploadedVideoData = videoData
            .Select(vd => new UploadedVideoData(vd.Key!, vd.Title, vd.UploadedAt!.Value))
            .ToList();
        
        return Result<List<UploadedVideoData>>.Success(uploadedVideoData);
    }

    public async Task<Result<Guid>> UpdateVideoDataAsync(Guid id, string title, string description, 
        string thumbnailUrl, CancellationToken cancellationToken)
    {
        var rowsUpdated = await _videoDataRepository.UpdateAsync(id, title, description, 
            thumbnailUrl, cancellationToken);

        if (rowsUpdated == 0)
        {
            return Result<Guid>.Failure(VideoDataError.NotFound);
        }
        
        return Result<Guid>.Success(id);
    }

    public async Task<Result<Guid>> UpdateVideoStatusAsync(Guid id, VideoStatus status,
        CancellationToken cancellationToken)
    {
        var rowsUpdated = await _videoDataRepository.UpdateStatusAsync(id, status, cancellationToken);

        if (rowsUpdated == 0)
        {
            return Result<Guid>.Failure(VideoDataError.NotFound);
        }
        
        return Result<Guid>.Success(id);
    }

    public async Task<Result<Guid>> SetVideoKeyAsync(Guid id, string key,
        CancellationToken cancellationToken)
    {
        var rowsUpdated = await _videoDataRepository.SetKeyAsync(id, key, cancellationToken);

        if (rowsUpdated == 0)
        {
            return Result<Guid>.Failure(VideoDataError.NotFound);
        }
        
        rowsUpdated = await _videoDataRepository.SetUploadDateAsync(id, cancellationToken);
        
        if (rowsUpdated == 0)
        {
            return Result<Guid>.Failure(VideoDataError.NotFound);
        }
        
        return Result<Guid>.Success(id);
    }


    public async Task<Result<Guid>> DeleteVideoDataAsync(Guid id, CancellationToken cancellationToken)
    {
        var rowsDeleted = await _videoDataRepository.DeleteAsync(id, cancellationToken);

        if (rowsDeleted == 0)
        {
            return Result<Guid>.Failure(VideoDataError.NotFound);
        }

        return Result<Guid>.Success(id);
    }
    
}