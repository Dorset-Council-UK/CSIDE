using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.Infrastructure;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal class InfrastructureItemConfiguration : IEntityTypeConfiguration<InfrastructureItem>
    {
        public void Configure(EntityTypeBuilder<InfrastructureItem> builder)
        {
            builder.Property(x => x.Description).HasMaxLength(1000);
            builder.Property(x => x.Geom).IsRequired().HasColumnType("geometry (point)");

            builder.Navigation(x => x.InfrastructureType)
                .AutoInclude();

            builder
                .Navigation(x => x.InfrastructureMedia)
                .AutoInclude();

            builder
               .Navigation(x => x.MaintenanceTeam)
               .AutoInclude();

            builder
                .Navigation(x => x.Parish)
                .AutoInclude();

            builder
                .Navigation(x => x.BridgeDetails)
                .AutoInclude();

            builder.HasOne(j => j.Parish).WithMany().HasForeignKey(j => j.ParishId).HasPrincipalKey(p => p.ParishId);
        }
    }
}
