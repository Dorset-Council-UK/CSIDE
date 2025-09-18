using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.DMMO;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal sealed class DMMOLinkedRouteConfiguration : IEntityTypeConfiguration<DMMOLinkedRoute>
    {
        public void Configure(EntityTypeBuilder<DMMOLinkedRoute> builder)
        {
            builder.HasKey(x => new { x.DMMOApplicationId, x.RouteId });

            builder.Navigation(x => x.Route)
                .AutoInclude();
        }
    }
}
