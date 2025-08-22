using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.PPO;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal sealed class PPOParishConfiguration : IEntityTypeConfiguration<PPOParish>
    {
        public void Configure(EntityTypeBuilder<PPOParish> builder)
        {
            builder.HasKey(x => new { x.ApplicationId, x.ParishId });

            builder.Navigation(x => x.Parish)
                .AutoInclude();
        }
    }
}
