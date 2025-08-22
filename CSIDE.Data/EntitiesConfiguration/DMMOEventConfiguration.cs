using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.DMMO;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal sealed class DMMOEventConfiguration : IEntityTypeConfiguration<DMMOEvent>
    {
        public void Configure(EntityTypeBuilder<DMMOEvent> builder)
        {
            builder
                .Property(x => x.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(x => x.EventDate)
                .IsRequired();
        }
    }
}
