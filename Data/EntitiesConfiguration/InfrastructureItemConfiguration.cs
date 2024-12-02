using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.Infrastructure;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal class InfrastructureItemConfiguration : IEntityTypeConfiguration<InfrastructureItem>
    {
        public void Configure(EntityTypeBuilder<InfrastructureItem> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(200);
            builder.Property(x => x.Description).HasMaxLength(1000);
            builder.Property(x => x.Geom).IsRequired().HasColumnType("geometry (point)");

            builder.Navigation(x => x.InfrastructureType)
                .AutoInclude();
        }
    }
}
