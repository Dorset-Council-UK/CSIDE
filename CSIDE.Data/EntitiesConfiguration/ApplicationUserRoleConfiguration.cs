using CSIDE.Data.Models.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal sealed class ApplicationUserRoleConfiguration : IEntityTypeConfiguration<ApplicationUserRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserRole> builder)
        {
            builder
                .HasKey(o => new { o.UserId, o.ApplicationRoleId });
        }
    }
}
