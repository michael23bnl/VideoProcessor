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

    public async Task<Result<Guid>> CreateVideoMetadataAsync(
        Guid id,
        string title,
        string? description,
        VideoStatus status,
        string? thumbnailUrl, 
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(title))
            return Result<Guid>.Failure(VideoDataError.InvalidTitle);

        /*if (string.IsNullOrEmpty(thumbnailUrl))
            return Result<Guid>.Failure(VideoDataError.InvalidThumbnailUrl);*/

        var videoMetadataId = await _videoDataRepository
            .CreateMetadataAsync(id, title, description, status, thumbnailUrl, cancellationToken);
    
        return Result<Guid>.Success(videoMetadataId);
    }
    
    public async Task<Result<Guid>> CreateVideoManifestAsync(Guid id, string protocol, string s3Key,
        CancellationToken cancellationToken)
    {
        var manifestId = await _videoDataRepository.CreateManifestAsync(id, protocol, s3Key, cancellationToken);
        var rowsUpdated = await _videoDataRepository.SetUploadDateAsync(id, cancellationToken);
        
        if (rowsUpdated == 0)
        {
            return Result<Guid>.Failure(VideoDataError.NotFound);
        }
        
        return Result<Guid>.Success(id);
    }

    public async Task<Result<VideoMetadata>> GetVideoDataAsync(Guid id, CancellationToken cancellationToken)
    {
        var videoData = await _videoDataRepository.GetAsync(id, cancellationToken);

        if (videoData is null)
        {
            return Result<VideoMetadata>.Failure(VideoDataError.NotFound);
        }
        
        return Result<VideoMetadata>.Success(videoData);
    }

    // public async Task<Result<List<VideoMetadata>>> GetAllVideoDataAsync(CancellationToken cancellationToken)
    // {
    //     var videoData = await _videoDataRepository.GetAllFilteredAsync(null, cancellationToken);
    //
    //     return Result<List<VideoMetadata>>.Success(videoData);
    // }

    public async Task<Result<List<UploadedVideoData>>> GetUploadedVideoDataAsync(CancellationToken cancellationToken)
    {
        var videoData = await _videoDataRepository.GetAllFilteredAsync(
            x => x.Status == VideoStatus.Ready, cancellationToken);
        
        var uploadedVideoData = videoData
            .Select(vm => new UploadedVideoData(
                vm.Manifests
                    .Select(m => new VideoManifestInfo(
                        m.Protocol, 
                        m.S3Key))
                    .ToList(), 
                vm.Title, 
                vm.UploadedAt!.Value))
            .ToList();
        
        return Result<List<UploadedVideoData>>.Success(uploadedVideoData);
    }

    public async Task<Result<Guid>> UpdateVideoMetadataAsync(Guid id, string title, string? description, 
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