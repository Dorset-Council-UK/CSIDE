using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal class StatementConfiguration : IEntityTypeConfiguration<Models.RightsOfWay.Statement>
    {
        public void Configure(EntityTypeBuilder<Models.RightsOfWay.Statement> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.RouteId).IsRequired();
            builder.Property(x => x.StatementText).IsRequired();
            builder.Property(x => x.Version).IsRequired();
            builder.Property(x => x.CreatedDate).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(x => x.StartGridRef).HasMaxLength(20);
            builder.Property(x => x.EndGridRef).HasMaxLength(20);
        }
    }
}
