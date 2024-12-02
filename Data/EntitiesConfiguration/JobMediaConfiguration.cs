using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.Shared;
using CSIDE.Data.Models.Maintenance;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal class JobMediaConfiguration : IEntityTypeConfiguration<JobMedia>
    {
        public void Configure(EntityTypeBuilder<JobMedia> builder)
        {
            builder.HasKey(x => new { x.JobId, x.MediaId });

            builder.Navigation(x => x.Media)
                .AutoInclude();
        }
    }
}
