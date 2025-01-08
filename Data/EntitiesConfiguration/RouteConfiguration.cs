using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.Maintenance;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal class RouteConfiguration : IEntityTypeConfiguration<Models.RightsOfWay.Route>
    {
        public void Configure(EntityTypeBuilder<Models.RightsOfWay.Route> builder)
        {
            builder.Property(x => x.RouteCode).ValueGeneratedNever();
            builder.HasKey(x => x.RouteCode);
            builder.Property(x => x.OperationalStatusId).IsRequired();
            builder.Property(x => x.LegalStatusId).IsRequired();
            builder.Property(x => x.RouteTypeId).IsRequired();
            builder.Property(x => x.Geom).IsRequired().HasColumnType("geometry (multilinestring)");

            builder
                .Navigation(x => x.OperationalStatus)
                .AutoInclude();
            builder
                .Navigation(x => x.LegalStatus)
                .AutoInclude();
            builder
                .Navigation(x => x.RouteType)
                .AutoInclude();
            builder
                .Navigation(x => x.MaintenanceTeam)
                .AutoInclude();
            builder
                .Navigation(x => x.Parish)
                .AutoInclude();

            builder
                .Navigation(x => x.RouteMedia)
                .AutoInclude();
            builder.
                Navigation(x => x.Statements)
                .AutoInclude();

            builder.HasOne(x => x.Parish).WithMany().HasForeignKey(x => x.ParishId).HasPrincipalKey(p => p.ParishId);
        }
    }
}
