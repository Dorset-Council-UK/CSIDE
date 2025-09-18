using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.DMMO;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal sealed class DMMOContactConfiguration : IEntityTypeConfiguration<DMMOContact>
    {
        public void Configure(EntityTypeBuilder<DMMOContact> builder)
        {
            builder.HasKey(x => new { x.DMMOApplicationId, x.ContactId });

            builder.Navigation(x => x.Contact)
                .AutoInclude();
        }
    }
}
