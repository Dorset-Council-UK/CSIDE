using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.DMMO;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal class DMMOParishConfiguration : IEntityTypeConfiguration<DMMOParish>
    {
        public void Configure(EntityTypeBuilder<DMMOParish> builder)
        {
            builder.HasKey(x => new { x.ApplicationId, x.ParishId });

            builder.Navigation(x => x.Parish)
                .AutoInclude();
        }
    }
}
