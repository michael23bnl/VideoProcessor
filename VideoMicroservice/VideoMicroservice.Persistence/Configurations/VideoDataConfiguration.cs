using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoMicroservice.VideoMicroservice.Core.Models;

namespace VideoMicroservice.VideoMicroservice.Persistence.Configurations;

public class VideoDataConfiguration : IEntityTypeConfiguration<VideoData>
{

    public void Configure(EntityTypeBuilder<VideoData> builder)
    {
        builder.HasKey(vd => vd.Id);
        builder.Property(vd => vd.Title).IsRequired();
        builder.Property(vd => vd.Url).IsRequired();
        builder.Property(vd => vd.ThumbnailUrl).IsRequired();
        builder.Property(vd => vd.Description).IsRequired();
    }
}