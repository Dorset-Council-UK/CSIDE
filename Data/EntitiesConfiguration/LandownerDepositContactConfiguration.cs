using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.LandownerDeposits;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal class LandownerDepositContactConfiguration : IEntityTypeConfiguration<LandownerDepositContact>
    {
        public void Configure(EntityTypeBuilder<LandownerDepositContact> builder)
        {
            builder.HasKey(x => new { x.LandownerDepositId, x.ContactId });

            builder.Navigation(x => x.Contact)
                .AutoInclude();
        }
    }
}
