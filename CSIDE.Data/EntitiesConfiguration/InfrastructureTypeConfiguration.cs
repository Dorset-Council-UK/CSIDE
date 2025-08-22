using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.Infrastructure;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal sealed class InfrastructureTypeConfiguration : IEntityTypeConfiguration<InfrastructureType>
    {
        public void Configure(EntityTypeBuilder<InfrastructureType> builder)
        {
            builder.Property(x => x.Name).IsRequired().HasMaxLength(500);
            builder.Property(x => x.Description).HasMaxLength(1000);
        }
    }
}
