using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.Maintenance;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal sealed class JobProblemTypeConfiguration : IEntityTypeConfiguration<JobProblemType>
    {
        public void Configure(EntityTypeBuilder<JobProblemType> builder)
        {
            builder.HasKey(x => new { x.JobId, x.ProblemTypeId });

            builder.Navigation(x => x.ProblemType)
                .AutoInclude();
        }
    }
}
