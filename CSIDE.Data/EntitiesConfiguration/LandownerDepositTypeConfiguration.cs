using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.LandownerDeposits;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal sealed class LandownerDepositTypeConfiguration : IEntityTypeConfiguration<LandownerDepositType>
    {
        public void Configure(EntityTypeBuilder<LandownerDepositType> builder)
        {
            builder.HasKey(x => new { x.LandownerDepositId, x.LandownerDepositSecondaryId, x.LandownerDepositTypeNameId });

            builder.Navigation(x => x.LandownerDepositTypeName)
                .AutoInclude();
        }
    }
}
