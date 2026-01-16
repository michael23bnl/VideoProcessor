using Microsoft.EntityFrameworkCore;
using VideoDataMicroservice.Core.Models;
using VideoDataMicroservice.Infrastructure.Configurations;

namespace VideoDataMicroservice.Infrastructure;

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