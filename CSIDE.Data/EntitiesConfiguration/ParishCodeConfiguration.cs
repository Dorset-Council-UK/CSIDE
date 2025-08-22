using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.Shared;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal class ParishCodeConfiguration : IEntityTypeConfiguration<ParishCode>
    {
        public void Configure(EntityTypeBuilder<ParishCode> builder)
        {
            builder.Property(x => x.ParishId).IsRequired();
            builder.Property(x => x.Code).IsRequired().HasMaxLength(20);

            //define composite key
            builder.HasKey(x => new { x.ParishId, x.Code });

            builder
                .HasOne(x => x.Parish)
                .WithMany()
                .HasForeignKey(x => x.ParishId)
                .HasPrincipalKey(p => p.ParishId);

        }
    }
}
