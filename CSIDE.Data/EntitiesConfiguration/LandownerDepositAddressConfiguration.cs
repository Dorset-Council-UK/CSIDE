using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.LandownerDeposits;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal sealed class LandownerDepositAddressConfiguration : IEntityTypeConfiguration<LandownerDepositAddress>
    {
        public void Configure(EntityTypeBuilder<LandownerDepositAddress> builder)
        {
            builder.HasKey(x => new { x.LandownerDepositId, x.LandownerDepositSecondaryId, x.UPRN });
        }
    }
}
