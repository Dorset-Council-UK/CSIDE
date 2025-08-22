using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.PPO;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal sealed class PPOIntentConfiguration : IEntityTypeConfiguration<PPOIntent>
    {
        public void Configure(EntityTypeBuilder<PPOIntent> builder)
        {
            builder.HasKey(x => new { x.ApplicationId, x.IntentId });

            builder.Navigation(x => x.Intent)
                .AutoInclude();
        }
    }
}
