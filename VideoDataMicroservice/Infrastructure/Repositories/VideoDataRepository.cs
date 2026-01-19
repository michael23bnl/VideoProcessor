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

    public async Task<Guid> CreateAsync(Guid id, string title, string? description, VideoStatus status,
        string? key, string? thumbnailUrl, CancellationToken cancellationToken)
    {
        var videoData = new VideoData
        {
            Id = id,
            Title = title,
            Description = description,
            Status = status,
            Key = key,
            ThumbnailUrl = thumbnailUrl
        };
        
        await _dbContext.VideoData.AddAsync(videoData, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return videoData.Id;
    }

    public async Task<VideoData?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var videoData = await _dbContext.VideoData.FindAsync(id, cancellationToken);
        
        return videoData;
    }

    /*public async Task<List<VideoData>> GetAllAsync(CancellationToken cancellationToken)
    {
        var videoData = await _dbContext.VideoData.AsNoTracking().ToListAsync(cancellationToken);
        
        return videoData;
    }*/

    public async Task<List<VideoData>> GetAllFilteredAsync(Expression<Func<VideoData, bool>>? filter = null, 
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.VideoData.AsNoTracking().AsQueryable();

        if (filter is not null)
        {
            query = query.Where(filter);
        }
        
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<int> UpdateAsync(Guid id, string title, string? description,
        string thumbnailUrl, CancellationToken cancellationToken)
    {
        var rowsUpdated = await _dbContext.VideoData
            .Where(vd => vd.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(vd => vd.Title, title)
                .SetProperty(vd => vd.Description, description)
                .SetProperty(vd => vd.ThumbnailUrl, thumbnailUrl), cancellationToken);
        
        return rowsUpdated;
    }

    public async Task<int> UpdateStatusAsync(Guid id, VideoStatus status,
        CancellationToken cancellationToken)
    {
        var rawsUpdated = await _dbContext.VideoData
            .Where(vd => vd.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(vd => vd.Status, status), cancellationToken);
        
        return rawsUpdated;
    }

    public async Task<int> SetKeyAsync(Guid id, string key,
        CancellationToken cancellationToken)
    {
        var rawsUpdated = await _dbContext.VideoData
            .Where(vd => vd.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(vd => vd.Key, key), cancellationToken);
        
        return rawsUpdated;
    }
    
    public async Task<int> SetUploadDateAsync(Guid id,
        CancellationToken cancellationToken)
    {
        var rawsUpdated = await _dbContext.VideoData
            .Where(vd => vd.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(vd => vd.UploadedAt, DateTime.UtcNow), cancellationToken);
        
        return rawsUpdated;
    }

    public async Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var rowsDeleted = await _dbContext.VideoData
            .Where(vd => vd.Id == id)
            .ExecuteDeleteAsync(cancellationToken);
        
        return rowsDeleted;
    }
}