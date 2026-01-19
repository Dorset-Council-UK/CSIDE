using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.DMMO;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal sealed class DMMOClaimedStatusConfiguration : IEntityTypeConfiguration<DMMOClaimedStatus>
    {
        public void Configure(EntityTypeBuilder<DMMOClaimedStatus> builder)
        {
            builder.HasKey(x => new { x.DMMOApplicationId, x.ClaimedStatusId });

            builder.Navigation(x => x.ClaimedStatus)
                .AutoInclude();
        }
    }
}
