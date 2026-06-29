using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoDataMicroservice.Core.Models;

namespace VideoDataMicroservice.Infrastructure.Configurations;

public class VideoMetadataConfiguration : IEntityTypeConfiguration<VideoMetadata>
{
    public void Configure(EntityTypeBuilder<VideoMetadata> builder)
    {
        builder.HasKey(vm => vm.Id);
        
        builder.Property(vm => vm.Title).IsRequired();
        builder.Property(vm => vm.Description);
        builder.Property(vm => vm.UploadedAt);
        builder.Property(vm => vm.Status).IsRequired();
        builder.Property(vm => vm.ThumbnailUrl);
        
        builder.HasMany(vm => vm.Manifests)
            .WithOne(m => m.Metadata)
            .HasForeignKey(m => m.VideoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}