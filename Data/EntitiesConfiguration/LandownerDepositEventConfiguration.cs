using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.LandownerDeposits;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal class LandownerDepositEventConfiguration : IEntityTypeConfiguration<LandownerDepositEvent>
    {
        public void Configure(EntityTypeBuilder<LandownerDepositEvent> builder)
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
