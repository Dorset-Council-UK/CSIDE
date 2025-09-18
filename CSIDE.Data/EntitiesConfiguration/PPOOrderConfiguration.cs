using CSIDE.Data.Models.PPO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal sealed class PPOOrderConfiguration : IEntityTypeConfiguration<PPOOrder>
    {
        public void Configure(EntityTypeBuilder<PPOOrder> builder)
        {
            builder.HasKey(x => new { x.OrderId, x.PPOApplicationId });
            builder
                .Navigation(x => x.DeterminationProcess)
                .AutoInclude();

            builder
                .Navigation(x => x.DecisionOfSecState)
                .AutoInclude();
        }
    }
}
