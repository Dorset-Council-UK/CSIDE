using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.Infrastructure;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal class InfrastructureBridgeDetailsConfiguration : IEntityTypeConfiguration<InfrastructureBridgeDetails>
    {
        public void Configure(EntityTypeBuilder<InfrastructureBridgeDetails> builder)
        {

            builder.Property(x => x.BeamTimbersSize)
                .HasMaxLength(20);
            builder.Property(x => x.DeckingBoardsSize)
                .HasMaxLength(20);
            builder.Property(x => x.HandrailPostsTimbersSize)
                .HasMaxLength(20);
            builder.Property(x => x.HandrailTimbersSize)
                .HasMaxLength(20);
            builder.Property(x => x.DeckingBoardsLength).HasPrecision(3, 1);

            builder.Navigation(x => x.Infrastructure)
                .AutoInclude();
            builder.Navigation(x => x.BeamMaterial)
                .AutoInclude();
            builder.Navigation(x => x.DeckingMaterial)
                .AutoInclude();
            builder.Navigation(x => x.HandrailMaterial)
                .AutoInclude();
            builder.Navigation(x => x.HandrailPostsMaterial)
                .AutoInclude();
            builder.Navigation(x => x.BankSeatMaterial)
                .AutoInclude();
            builder.Navigation(x => x.BeamCondition)
                .AutoInclude();
            builder.Navigation(x => x.DeckingCondition)
                .AutoInclude();
            builder.Navigation(x => x.HandrailCondition)
                .AutoInclude();
            builder.Navigation(x => x.HandrailPostsCondition)
                .AutoInclude();
            builder.Navigation(x => x.BankSeatCondition)
                .AutoInclude();
        }
    }
}
