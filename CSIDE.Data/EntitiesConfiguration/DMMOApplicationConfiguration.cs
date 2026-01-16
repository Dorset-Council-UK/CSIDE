using CSIDE.Data.Models.DMMO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal sealed class DMMOApplicationConfiguration : IEntityTypeConfiguration<DMMOApplication>
    {
        public void Configure(EntityTypeBuilder<DMMOApplication> builder)
        {

            builder.Property(x => x.ApplicationDetails).IsRequired();
            builder.Property(x => x.CaseStatusId).IsRequired();
            builder.Property(x => x.Geom).IsRequired().HasColumnType("geometry (multilinestring)");
            builder.Property(x => x.Version).IsRowVersion();

            builder
                .Navigation(x => x.CaseStatus)
                .AutoInclude();

            builder
                .Navigation(x => x.Orders)
                .AutoInclude();

            builder
                .Navigation(x => x.Events)
                .AutoInclude();

            builder
                .Navigation(x => x.DirectionOfSecState)
                .AutoInclude();

            builder
                .Navigation(x => x.DMMOMedia)
                .AutoInclude();

            builder
                .Navigation(x => x.DMMOContacts)
                .AutoInclude();

            builder
                .Navigation(x => x.DMMOParishes)
                .AutoInclude();

            builder
                .Navigation(x => x.DMMOAddresses)
                .AutoInclude();

            builder
                .Navigation(x => x.DMMOLinkedRoutes)
                .AutoInclude();

            builder
                .Navigation(x => x.DMMOClaimedStatuses)
                .AutoInclude();

            builder
                .Navigation(x => x.DMMOApplicationTypes)
                .AutoInclude();
        }
    }
}
