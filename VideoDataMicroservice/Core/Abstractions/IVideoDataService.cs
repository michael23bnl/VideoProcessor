using Shared.Enums;
using Shared.Result;
using VideoDataMicroservice.Application.DTO;
using VideoDataMicroservice.Core.Models;

namespace VideoDataMicroservice.Core.Abstractions;

public interface IVideoDataService
{
    Task<Result<Guid>> CreateVideoDataAsync(Guid id, string title, string? description, VideoStatus status,
        string? key, string? thumbnailUrl, CancellationToken cancellationToken);
    Task<Result<VideoData>> GetVideoDataAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<List<VideoData>>> GetAllVideoDataAsync(CancellationToken cancellationToken);
    Task<Result<List<UploadedVideoData>>> GetUploadedVideoDataAsync(CancellationToken cancellationToken);
    Task<Result<Guid>> UpdateVideoDataAsync(Guid id, string title, string description, 
        string thumbnailUrl, CancellationToken cancellationToken);
    Task<Result<Guid>> UpdateVideoStatusAsync(Guid id, VideoStatus status, CancellationToken cancellationToken);
    Task<Result<Guid>> SetVideoKeyAsync(Guid id, string key, CancellationToken cancellationToken);
    Task<Result<Guid>> DeleteVideoDataAsync(Guid id, CancellationToken cancellationToken);
}