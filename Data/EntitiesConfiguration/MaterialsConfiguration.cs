using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.Surveys;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal class MaterialsConfiguration : IEntityTypeConfiguration<Material>
    {
        public void Configure(EntityTypeBuilder<Material> builder)
        {
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.IsWood)
                .IsRequired()
                .HasDefaultValue(false);
        }
    }
}
