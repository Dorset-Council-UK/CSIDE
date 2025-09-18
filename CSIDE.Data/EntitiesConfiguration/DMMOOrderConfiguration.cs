using CSIDE.Data.Models.DMMO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal sealed class DMMOOrderConfiguration : IEntityTypeConfiguration<DMMOOrder>
    {
        public void Configure(EntityTypeBuilder<DMMOOrder> builder)
        {
            builder.HasKey(x => new { x.OrderId, x.DMMOApplicationId });
            builder
                .Navigation(x => x.DeterminationProcess)
                .AutoInclude();

            builder
                .Navigation(x => x.DecisionOfSecState)
                .AutoInclude();
        }
    }
}
