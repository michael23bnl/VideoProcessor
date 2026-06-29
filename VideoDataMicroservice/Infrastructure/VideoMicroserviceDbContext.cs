using Microsoft.EntityFrameworkCore;
using VideoDataMicroservice.Core.Models;
using VideoDataMicroservice.Infrastructure.Configurations;

namespace VideoDataMicroservice.Infrastructure;

public class VideoMicroserviceDbContext : DbContext
{
    public VideoMicroserviceDbContext(DbContextOptions<VideoMicroserviceDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<VideoMetadata> VideoMetadata { get; set; }
    public DbSet<VideoManifest> VideoManifest { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new VideoMetadataConfiguration());
    
        base.OnModelCreating(modelBuilder);
    }
}