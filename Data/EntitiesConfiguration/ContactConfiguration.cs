using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.Shared;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal class ContactConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(500);
            builder.Property(x => x.Email).HasMaxLength(500);
            builder.Property(x => x.PrimaryContactNo).HasMaxLength(20);
            builder.Property(x => x.SecondaryContactNo).HasMaxLength(20);
            builder.Property(x => x.OrganisationName).HasMaxLength(500);

            builder.Navigation(x => x.ContactType)
                .AutoInclude();
        }
    }
}
