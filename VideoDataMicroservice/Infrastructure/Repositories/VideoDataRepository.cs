using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;
using VideoDataMicroservice.Core.Abstractions;
using VideoDataMicroservice.Core.Models;

namespace VideoDataMicroservice.Infrastructure.Repositories;

public class VideoDataRepository : IVideoDataRepository
{
    private readonly VideoMicroserviceDbContext _dbContext;
    
    public VideoDataRepository(VideoMicroserviceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> CreateMetadataAsync(Guid id, string title, string? description, VideoStatus status,
        string? thumbnailUrl, CancellationToken cancellationToken)
    {
        var videoData = new VideoMetadata
        {
            Id = id,
            Title = title,
            Description = description,
            Status = status,
            ThumbnailUrl = thumbnailUrl,
        };
        
        await _dbContext.VideoMetadata.AddAsync(videoData, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return videoData.Id;
    }

    public async Task<Guid> CreateManifestAsync(Guid videoId, string protocol, string s3Key, CancellationToken cancellationToken)
    {
        var manifest = new VideoManifest
        {
            Id = Guid.NewGuid(),
            VideoId = videoId,
            Protocol = protocol,
            S3Key = s3Key
        };
        
        await _dbContext.VideoManifest.AddAsync(manifest, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return manifest.Id;
    }

    public async Task<VideoMetadata?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var videoData = await _dbContext.VideoMetadata
            .Include(vm => vm.Manifests)
            .FirstOrDefaultAsync(vm => vm.Id == id, cancellationToken);
        
        return videoData;
    }

    public async Task<List<VideoMetadata>> GetAllFilteredAsync(Expression<Func<VideoMetadata, bool>>? filter = null, 
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.VideoMetadata
            .Include(vm => vm.Manifests)
            .AsNoTracking()
            .AsQueryable();

        if (filter is not null)
        {
            query = query.Where(filter);
        }
        
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<int> UpdateAsync(Guid id, string title, string? description,
        string thumbnailUrl, CancellationToken cancellationToken)
    {
        var rowsUpdated = await _dbContext.VideoMetadata
            .Where(vm => vm.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(vm => vm.Title, title)
                .SetProperty(vm => vm.Description, description)
                .SetProperty(vm => vm.ThumbnailUrl, thumbnailUrl), cancellationToken);
        
        return rowsUpdated;
    }

    public async Task<int> UpdateStatusAsync(Guid id, VideoStatus status,
        CancellationToken cancellationToken)
    {
        var rawsUpdated = await _dbContext.VideoMetadata
            .Where(vm => vm.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(vm => vm.Status, status), cancellationToken);
        
        return rawsUpdated;
    }
    
    public async Task<int> SetUploadDateAsync(Guid id,
        CancellationToken cancellationToken)
    {
        var rawsUpdated = await _dbContext.VideoMetadata
            .Where(vm => vm.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(vm => vm.UploadedAt, DateTime.UtcNow), cancellationToken);
        
        return rawsUpdated;
    }

    public async Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var rowsDeleted = await _dbContext.VideoMetadata
            .Where(vm => vm.Id == id)
            .ExecuteDeleteAsync(cancellationToken);
        
        return rowsDeleted;
    }
}