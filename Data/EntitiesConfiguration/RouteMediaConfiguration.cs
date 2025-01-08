using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.RightsOfWay;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal class RouteMediaConfiguration : IEntityTypeConfiguration<RouteMedia>
    {
        public void Configure(EntityTypeBuilder<RouteMedia> builder)
        {
            builder.HasKey(x => new { x.RouteId, x.MediaId });

            builder.Navigation(x => x.Media)
                .AutoInclude();
        }
    }
}
