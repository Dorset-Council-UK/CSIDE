using CSIDE.Data.Models.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.Maintenance;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal class MaintenanceJobConfiguration : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            builder
                .Property(x => x.LogDate)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(x => x.ProblemDescription)
                .HasMaxLength(4000)
                .IsRequired();
            builder.Property(x => x.JobPriorityId).IsRequired();
            builder.Property(x => x.JobStatusId).IsRequired();
            builder.Property(x => x.WorkDone)
                .HasMaxLength(4000);
            builder.Property(x => x.Geom).IsRequired().HasColumnType("geometry (point)");

            builder
                .Navigation(x => x.JobPriority)
                .AutoInclude();

            builder
                .Navigation(x => x.JobStatus)
                .AutoInclude();

            builder
                .Navigation(x => x.Route)
                .AutoInclude();
        }
    }
}
