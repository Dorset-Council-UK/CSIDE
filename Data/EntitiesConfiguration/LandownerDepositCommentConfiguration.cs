using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.LandownerDeposits;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal class LandownerDepositCommentConfiguration : IEntityTypeConfiguration<LandownerDepositComment>
    {
        public void Configure(EntityTypeBuilder<LandownerDepositComment> builder)
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
