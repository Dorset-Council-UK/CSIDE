using CSIDE.Data.Models.RoW;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.Maintenance;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal class RouteConfiguration : IEntityTypeConfiguration<Models.RoW.Route>
    {
        public void Configure(EntityTypeBuilder<Models.RoW.Route> builder)
        {
            builder.Property(x => x.RouteCode).ValueGeneratedNever();
            builder.HasKey(x => x.RouteCode);
            builder.Property(x => x.Geom).IsRequired().HasColumnType("geometry (multilinestring)");
        }
    }
}
