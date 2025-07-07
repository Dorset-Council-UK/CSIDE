using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.DMMO;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal class DMMOCommentConfiguration : IEntityTypeConfiguration<DMMOComment>
    {
        public void Configure(EntityTypeBuilder<DMMOComment> builder)
        {
            builder
                .Property(x => x.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(x => x.CommentDate)
                .IsRequired();
        }
    }
}
