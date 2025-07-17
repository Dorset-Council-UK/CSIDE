using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.Infrastructure;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal class InfrastructureMediaConfiguration : IEntityTypeConfiguration<InfrastructureMedia>
    {
        public void Configure(EntityTypeBuilder<InfrastructureMedia> builder)
        {
            builder.HasKey(x => new { x.InfrastructureItemId, x.MediaId });

            builder.Navigation(x => x.Media)
                .AutoInclude();
        }
    }
}
