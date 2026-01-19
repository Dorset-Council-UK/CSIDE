using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.DMMO;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal sealed class DMMOApplicationTypeConfiguration : IEntityTypeConfiguration<DMMOApplicationType>
    {
        public void Configure(EntityTypeBuilder<DMMOApplicationType> builder)
        {
            builder.HasKey(x => new { x.DMMOApplicationId, x.ApplicationTypeId });

            builder.Navigation(x => x.ApplicationType)
                .AutoInclude();
        }
    }
}
