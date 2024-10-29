using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.Maintenance;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal class MaintenanceTeamConfiguration : IEntityTypeConfiguration<MaintenanceTeam>
    {
        public void Configure(EntityTypeBuilder<MaintenanceTeam> builder)
        {
            builder.Property(x => x.Geom).IsRequired().HasColumnType("geometry (polygon)");
        }
    }
}
