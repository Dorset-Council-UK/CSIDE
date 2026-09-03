using CSIDE.Data.Models.LandownerDeposits;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal sealed class LandownerDepositConfiguration : IEntityTypeConfiguration<LandownerDeposit>
    {
        public void Configure(EntityTypeBuilder<LandownerDeposit> builder)
        {
            builder.HasKey(x => new { x.Id, x.SecondaryId });

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Geom).IsRequired().HasColumnType("geometry(multipolygon, 27700)");

            builder.Property(x => x.Version)
                .IsRowVersion();

            builder
                .Navigation(x => x.LandownerDepositTypes)
                .AutoInclude();

            builder
               .Navigation(x => x.Events)
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
