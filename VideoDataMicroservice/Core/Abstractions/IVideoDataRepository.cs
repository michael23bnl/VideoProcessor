using System.Linq.Expressions;
using Shared.Enums;
using VideoDataMicroservice.Core.Models;

namespace VideoDataMicroservice.Core.Abstractions;

public interface IVideoDataRepository
{
    Task<Guid> CreateAsync(Guid id, string title, string? description, VideoStatus status, 
        string? key, string? thumbnailUrl, CancellationToken cancellationToken);
    Task<VideoData?> GetAsync(Guid id, CancellationToken cancellationToken);
    //Task<List<VideoData>> GetAllAsync(CancellationToken cancellationToken);
    Task<List<VideoData>> GetAllFilteredAsync(Expression<Func<VideoData, bool>>? filter = null, 
        CancellationToken cancellationToken = default);
    Task<int> UpdateAsync(Guid id, string title, string? description,
        string thumbnailUrl, CancellationToken cancellationToken);
    Task<int> UpdateStatusAsync(Guid id, VideoStatus status,
        CancellationToken cancellationToken);
    Task<int> SetKeyAsync(Guid id, string key,
        CancellationToken cancellationToken);
    Task<int> SetUploadDateAsync(Guid id,
        CancellationToken cancellationToken);
    Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken);
}