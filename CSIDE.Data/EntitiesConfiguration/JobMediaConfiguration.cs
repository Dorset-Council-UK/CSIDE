using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.Maintenance;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal sealed class JobMediaConfiguration : IEntityTypeConfiguration<JobMedia>
    {
        public void Configure(EntityTypeBuilder<JobMedia> builder)
        {
            builder.HasKey(x => new { x.JobId, x.MediaId });

            builder.Navigation(x => x.Media)
                .AutoInclude();
        }
    }
}
