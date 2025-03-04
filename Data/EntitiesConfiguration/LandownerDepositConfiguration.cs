using CSIDE.Data.Models.LandownerDeposits;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal class LandownerDepositConfiguration : IEntityTypeConfiguration<LandownerDeposit>
    {
        public void Configure(EntityTypeBuilder<LandownerDeposit> builder)
        {
            builder.Property(x => x.Geom).IsRequired().HasColumnType("geometry (multipolygon)");

            builder.Property(x => x.Version)
                .IsRowVersion();

            builder
                .Navigation(x => x.LandownerDepositTypes)
                .AutoInclude();

            builder
                .Navigation(x => x.LandownerDepositContacts)
                .AutoInclude();

            builder
                .Navigation(x => x.LandownerDepositMedia)
                .AutoInclude();

            builder 
                .Navigation(x => x.LandownerDepositAddresses)
                .AutoInclude();

            builder
                .Navigation(x => x.LandownerDepositParishes)
                .AutoInclude();
        }
    }
}
