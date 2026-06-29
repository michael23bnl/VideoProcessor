using Shared.Enums;
using Shared.Result;
using VideoDataMicroservice.Application.DTO;
using VideoDataMicroservice.Core.Models;

namespace VideoDataMicroservice.Core.Abstractions;

public interface IVideoDataService
{
    Task<Result<Guid>> CreateVideoMetadataAsync(Guid id, string title, string? description, VideoStatus status,
        string? thumbnailUrl, CancellationToken cancellationToken);
    Task<Result<Guid>> CreateVideoManifestAsync(Guid id, string protocol, string s3Key,
        CancellationToken cancellationToken);
    Task<Result<VideoMetadata>> GetVideoDataAsync(Guid id, CancellationToken cancellationToken);
    //Task<Result<List<VideoMetadata>>> GetAllVideoDataAsync(CancellationToken cancellationToken);
    Task<Result<List<UploadedVideoData>>> GetUploadedVideoDataAsync(CancellationToken cancellationToken);
    Task<Result<Guid>> UpdateVideoMetadataAsync(Guid id, string title, string? description, 
        string thumbnailUrl, CancellationToken cancellationToken);
    Task<Result<Guid>> UpdateVideoStatusAsync(Guid id, VideoStatus status, CancellationToken cancellationToken);
    Task<Result<Guid>> DeleteVideoDataAsync(Guid id, CancellationToken cancellationToken);
}