using CSIDE.Data.Models.Maintenance;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal class JobInfrastructureConfiguration : IEntityTypeConfiguration<JobInfrastructure>
    {
        public void Configure(EntityTypeBuilder<JobInfrastructure> builder)
        {
            builder.HasKey(x => new { x.JobId, x.InfrastructureId });

            builder.Navigation(x => x.Infrastructure)
                .AutoInclude();
        }
    }
}
