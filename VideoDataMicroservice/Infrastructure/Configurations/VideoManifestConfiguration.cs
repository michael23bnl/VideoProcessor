using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoDataMicroservice.Core.Models;

namespace VideoDataMicroservice.Infrastructure.Configurations;

public class VideoManifestConfiguration : IEntityTypeConfiguration<VideoManifest>
{
    public void Configure(EntityTypeBuilder<VideoManifest> builder)
    {
        builder.HasKey(vm => vm.Id);
        
        builder.Property(vm => vm.Protocol);
        builder.Property(vm => vm.S3Key);
        
        builder.HasOne(vm => vm.Metadata)
            .WithMany(m => m.Manifests)
            .HasForeignKey(m => m.VideoId);
    }
}