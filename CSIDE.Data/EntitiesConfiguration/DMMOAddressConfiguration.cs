using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.DMMO;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal sealed class DMMOAddressConfiguration : IEntityTypeConfiguration<DMMOAddress>
    {
        public void Configure(EntityTypeBuilder<DMMOAddress> builder)
        {
            builder.HasKey(x => new { x.DMMOApplicationId, x.UPRN });
        }
    }
}
