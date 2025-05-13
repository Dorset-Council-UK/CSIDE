using CSIDE.Data.Models.DMMO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal class DMMOOrderConfiguration: IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(x => new { x.OrderId, x.ApplicationId });
            builder
                .Navigation(x => x.DeterminationProcess)
                .AutoInclude();

            builder
                .Navigation(x => x.DecisionOfSecState)
                .AutoInclude();
        }
    }
}
