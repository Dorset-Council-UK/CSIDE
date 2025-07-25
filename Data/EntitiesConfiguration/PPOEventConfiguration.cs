using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.PPO;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal class PPOEventConfiguration : IEntityTypeConfiguration<PPOEvent>
    {
        public void Configure(EntityTypeBuilder<PPOEvent> builder)
        {
            builder
                .Property(x => x.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(x => x.EventDate)
                .IsRequired();
        }
    }
}
