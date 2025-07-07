using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.PPO;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal class PPOMediaConfiguration : IEntityTypeConfiguration<PPOMedia>
    {
        public void Configure(EntityTypeBuilder<PPOMedia> builder)
        {
            builder.HasKey(x => new { x.PPOId, x.MediaId });

            builder.Navigation(x => x.Media)
                .AutoInclude();

            builder.Navigation(x => x.MediaType)
                .AutoInclude();
        }
    }
}
