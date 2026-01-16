using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoDataMicroservice.Core.Models;

namespace VideoDataMicroservice.Infrastructure.Configurations;

public class VideoDataConfiguration : IEntityTypeConfiguration<VideoData>
{
    public void Configure(EntityTypeBuilder<VideoData> builder)
    {
        builder.HasKey(vd => vd.Id);
        builder.Property(vd => vd.Title).IsRequired();
        builder.Property(vd => vd.Description);
        builder.Property(vd => vd.UploadedAt);
        builder.Property(vd => vd.Status).IsRequired();
        builder.Property(vd => vd.Key);
        builder.Property(vd => vd.ThumbnailUrl);
    }
}