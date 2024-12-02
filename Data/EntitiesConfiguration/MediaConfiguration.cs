using CSIDE.Data.Models.Shared;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal class MediaConfiguration : IEntityTypeConfiguration<Media>
    {
        public void Configure(EntityTypeBuilder<Media> builder)
        {
            builder
                .Property(x => x.UploadDate)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(x => x.URL).IsRequired();
            builder.Property(x => x.Title).HasMaxLength(200);
            builder.Ignore(x => x.Format);
        }
    }
}
