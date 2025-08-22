using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.Surveys;

namespace CSIDE.Data.EntitiesConfiguration
{
    internal sealed class SurveyMediaConfiguration : IEntityTypeConfiguration<SurveyMedia>
    {
        public void Configure(EntityTypeBuilder<SurveyMedia> builder)
        {
            builder.HasKey(x => new { x.SurveyId, x.MediaId });

            builder.Navigation(x => x.Media)
                .AutoInclude();
        }
    }
}
