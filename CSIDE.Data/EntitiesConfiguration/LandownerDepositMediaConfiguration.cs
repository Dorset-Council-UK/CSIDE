using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.LandownerDeposits;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal sealed class LandownerDepositMediaConfiguration : IEntityTypeConfiguration<LandownerDepositMedia>
    {
        public void Configure(EntityTypeBuilder<LandownerDepositMedia> builder)
        {
            builder.HasKey(x => new { x.LandownerDepositId, x.LandownerDepositSecondaryId, x.MediaId });

            builder.Navigation(x => x.Media)
                .AutoInclude();

            builder.Navigation(x => x.MediaType)
                .AutoInclude();
        }
    }
}
