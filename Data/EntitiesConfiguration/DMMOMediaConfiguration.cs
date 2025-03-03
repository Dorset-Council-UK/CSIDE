using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.DMMO;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal class DMMOMediaConfiguration : IEntityTypeConfiguration<DMMOMedia>
    {
        public void Configure(EntityTypeBuilder<DMMOMedia> builder)
        {
            builder.HasKey(x => new { x.DMMOId, x.MediaId });

            builder.Navigation(x => x.Media)
                .AutoInclude();

            builder.Navigation(x => x.MediaType)
                .AutoInclude();
        }
    }
}
