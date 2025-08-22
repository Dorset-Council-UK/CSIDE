using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.Maintenance;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal sealed class JobContactConfiguration : IEntityTypeConfiguration<JobContact>
    {
        public void Configure(EntityTypeBuilder<JobContact> builder)
        {
            builder.HasKey(x => new { x.JobId, x.ContactId });

            builder.Navigation(x => x.Contact)
                .AutoInclude();
        }
    }
}
