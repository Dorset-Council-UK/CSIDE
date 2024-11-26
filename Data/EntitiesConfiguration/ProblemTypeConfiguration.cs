using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.Shared;
using CSIDE.Data.Models.Maintenance;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal class ProblemTypeConfiguration : IEntityTypeConfiguration<ProblemType>
    {
        public void Configure(EntityTypeBuilder<ProblemType> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(100);
            builder.Property(x => x.Description).HasMaxLength(500);
        }
    }
}
