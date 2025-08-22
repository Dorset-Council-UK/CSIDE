using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.DMMO;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal sealed class DMMOContactConfiguration : IEntityTypeConfiguration<DMMOContact>
    {
        public void Configure(EntityTypeBuilder<DMMOContact> builder)
        {
            builder.HasKey(x => new { x.ApplicationId, x.ContactId });

            builder.Navigation(x => x.Contact)
                .AutoInclude();
        }
    }
}
