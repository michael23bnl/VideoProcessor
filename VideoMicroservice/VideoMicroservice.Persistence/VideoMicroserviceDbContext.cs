using Microsoft.EntityFrameworkCore;
using VideoMicroservice.VideoMicroservice.Core.Models;
using VideoMicroservice.VideoMicroservice.Persistence.Configurations;

namespace VideoMicroservice.VideoMicroservice.Persistence;

public class VideoMicroserviceDbContext : DbContext
{
    public VideoMicroserviceDbContext(DbContextOptions<VideoMicroserviceDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<VideoData> VideoData { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new VideoDataConfiguration());
    
        base.OnModelCreating(modelBuilder);
    }
}