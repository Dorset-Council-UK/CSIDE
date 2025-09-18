using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.PPO;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal sealed class PPOContactConfiguration : IEntityTypeConfiguration<PPOContact>
    {
        public void Configure(EntityTypeBuilder<PPOContact> builder)
        {
            builder.HasKey(x => new { x.PPOApplicationId, x.ContactId });

            builder.Navigation(x => x.Contact)
                .AutoInclude();
        }
    }
}
