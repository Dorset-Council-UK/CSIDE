using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.LandownerDeposits;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal class landownerDepositParishConfiguration : IEntityTypeConfiguration<LandownerDepositParish>
    {
        public void Configure(EntityTypeBuilder<LandownerDepositParish> builder)
        {
            builder.HasKey(x => new { x.LandownerDepositId, x.ParishId });

            builder.Navigation(x => x.Parish)
                .AutoInclude();
        }
    }
}
