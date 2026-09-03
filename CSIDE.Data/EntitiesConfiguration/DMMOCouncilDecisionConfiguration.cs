using CSIDE.Data.Models.DMMO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal sealed class DMMOCouncilDecisionConfiguration : IEntityTypeConfiguration<DMMOCouncilDecision>
    {
        public void Configure(EntityTypeBuilder<DMMOCouncilDecision> builder)
        {
            builder.HasKey(x => new { x.CouncilDecisionId, x.DMMOApplicationId });

            builder
                .Navigation(x => x.CouncilDecisionType)
                .AutoInclude();

            builder
                .Property(x => x.Date)
                .IsRequired();

            builder.Property(x => x.CouncilDecisionTypeId)
                .IsRequired();

        }
    }
}
