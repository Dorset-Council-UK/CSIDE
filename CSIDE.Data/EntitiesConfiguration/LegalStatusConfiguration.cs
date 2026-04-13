using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.RightsOfWay;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal sealed class LegalStatusConfiguration : IEntityTypeConfiguration<LegalStatus>
    {
        public void Configure(EntityTypeBuilder<LegalStatus> builder)
        {
            builder.Property(x => x.IsActive).HasDefaultValue(true);
        }
    }
}
