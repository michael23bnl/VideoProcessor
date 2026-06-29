using System.Linq.Expressions;
using Shared.Enums;
using VideoDataMicroservice.Core.Models;

namespace VideoDataMicroservice.Core.Abstractions;

public interface IVideoDataRepository
{
    Task<Guid> CreateMetadataAsync(Guid id, string title, string? description, VideoStatus status, 
        string? thumbnailUrl, CancellationToken cancellationToken);
    Task<Guid> CreateManifestAsync(Guid videoId, string protocol, string s3Key, CancellationToken cancellationToken);
    Task<VideoMetadata?> GetAsync(Guid id, CancellationToken cancellationToken);
    //Task<List<VideoData>> GetAllAsync(CancellationToken cancellationToken);
    Task<List<VideoMetadata>> GetAllFilteredAsync(Expression<Func<VideoMetadata, bool>>? filter = null, 
        CancellationToken cancellationToken = default);
    Task<int> UpdateAsync(Guid id, string title, string? description,
        string thumbnailUrl, CancellationToken cancellationToken);
    Task<int> UpdateStatusAsync(Guid id, VideoStatus status,
        CancellationToken cancellationToken);
    Task<int> SetUploadDateAsync(Guid id,
        CancellationToken cancellationToken);
    Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken);
}