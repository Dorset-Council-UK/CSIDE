using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.PPO;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal class PPOCommentConfiguration : IEntityTypeConfiguration<PPOComment>
    {
        public void Configure(EntityTypeBuilder<PPOComment> builder)
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
