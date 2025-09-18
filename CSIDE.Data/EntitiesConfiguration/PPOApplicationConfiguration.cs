using CSIDE.Data.Models.PPO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal sealed class PPOApplicationConfiguration : IEntityTypeConfiguration<PPOApplication>
    {
        public void Configure(EntityTypeBuilder<PPOApplication> builder)
        {

            builder.Property(x => x.ApplicationDetails).IsRequired();
            builder.Property(x => x.ApplicationTypeId).IsRequired();
            builder.Property(x => x.CaseStatusId).IsRequired();
            builder.Property(x => x.PriorityId).IsRequired();
            builder.Property(x => x.Geom).IsRequired().HasColumnType("geometry (multilinestring)");
            builder.Property(x => x.Version).IsRowVersion();

            builder
                .Navigation(x => x.ApplicationType)
                .AutoInclude();

            builder
                .Navigation(x => x.CaseStatus)
                .AutoInclude();

            builder
                .Navigation(x => x.Priority)
                .AutoInclude();

            builder
                .Navigation(x => x.Orders)
                .AutoInclude();


            builder.Navigation(x => x.Events)
                .AutoInclude();

            builder
                .Navigation(x => x.PPOMedia)
                .AutoInclude();

            builder
                .Navigation(x => x.PPOContacts)
                .AutoInclude();

            builder
                .Navigation(x => x.PPOParishes)
                .AutoInclude();

            builder.Navigation(x => x.PPOIntents)
                .AutoInclude();

        }
    }
}
