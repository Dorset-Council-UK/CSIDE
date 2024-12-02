using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.Shared;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal class ParishConfiguration : IEntityTypeConfiguration<Parish>
    {
        public void Configure(EntityTypeBuilder<Parish> builder)
        {
            builder.Property(x => x.ParishId).HasColumnName("feature_serial_number");
            builder.Property(x => x.Name).HasColumnName("name");
            builder.Property(x => x.Geom).HasColumnName("geom").IsRequired().HasColumnType("geometry (multipolygon)");

            builder.HasKey(x => x.ParishId);

            builder.ToTable("Parishes", p => p.ExcludeFromMigrations());
        }
    }
}
