using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.PPO;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal sealed class PPOTypeConfiguration : IEntityTypeConfiguration<PPOApplicationType>
    {
        public void Configure(EntityTypeBuilder<PPOApplicationType> builder)
        {
            builder.HasKey(x => new { x.PPOApplicationId, x.TypeId });

            builder.Navigation(x => x.Type)
                .AutoInclude();
        }
    }
}
