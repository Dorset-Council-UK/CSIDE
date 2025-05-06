using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.Maintenance;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal class MaintenanceTeamUserConfiguration : IEntityTypeConfiguration<TeamUser>
    {
        public void Configure(EntityTypeBuilder<TeamUser> builder)
        {
            builder.HasKey(x => new { x.TeamId, x.UserId });
            builder.Property(x => x.IsLead).HasDefaultValue(false);
        }
    }
}
