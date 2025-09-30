using CSIDE.Data.Models.Maintenance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Npgsql.EntityFrameworkCore.PostgreSQL.ValueGeneration;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal sealed class MaintenanceJobSubscriberConfiguration : IEntityTypeConfiguration<JobSubscriber>
    {
        public void Configure(EntityTypeBuilder<JobSubscriber> builder)
        {
            builder.HasKey(x => new { x.JobId, x.EmailAddress });
            builder.Property(x => x.UnsubscribeToken).HasValueGenerator<NpgsqlSequentialGuidValueGenerator>();
        }
    }
}
