using Microsoft.EntityFrameworkCore;
using VideoMicroservice.VideoMicroservice.Core.Models;
using VideoMicroservice.VideoMicroservice.Core.Abstractions;
namespace VideoMicroservice.VideoMicroservice.Persistence.Repositories;

public class VideoDataRepository : IVideoDataRepository
{
    private readonly VideoMicroserviceDbContext _dbContext;
    
    public VideoDataRepository(VideoMicroserviceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> CreateAsync(string title, string description, 
        string url, string thumbnailUrl)
    {
        var videoData = new VideoData
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            Url = url,
            ThumbnailUrl = thumbnailUrl
        };
        
        await _dbContext.VideoData.AddAsync(videoData);
        await _dbContext.SaveChangesAsync();
        
        return videoData.Id;
    }

    public async Task<VideoData?> GetAsync(Guid id)
    {
        var videoData = await _dbContext.VideoData.FindAsync(id);
        
        return videoData;
    }

    public async Task<List<VideoData>> GetAllAsync()
    {
        var videoData = await _dbContext.VideoData.AsNoTracking().ToListAsync();
        
        return videoData;
    }

    public async Task<int> UpdateAsync(Guid id, string title, string description,
        string thumbnailUrl)
    {
        var rowsUpdated = await _dbContext.VideoData
            .Where(vd => vd.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(vd => vd.Title, title)
                .SetProperty(vd => vd.Description, description)
                .SetProperty(vd => vd.ThumbnailUrl, thumbnailUrl));
        
        return rowsUpdated;
    }

    public async Task<int> DeleteAsync(Guid id)
    {
        var rowsDeleted = await _dbContext.VideoData
            .Where(vd => vd.Id == id)
            .ExecuteDeleteAsync();
        
        return rowsDeleted;
    }
}