using BlazorBootstrap;
using CSIDE.Data;
using CSIDE.Data.Models.PPO;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Globalization;

namespace CSIDE.Web.Components.Pages.PPO
{
    public partial class Applications(IDbContextFactory<ApplicationDbContext> contextFactory, ILogger<Applications> logger, IPlacesSearchService placesSearchService)
    {
        private List<BreadcrumbItem>? NavItems;


        [SupplyParameterFromQuery]
        private string[]? ParishIds { get; set; }
        [SupplyParameterFromQuery]
        private string? ParishId { get; set; }
        [SupplyParameterFromQuery]
        private string? Location { get; set; }
        [SupplyParameterFromQuery]
        private string? ApplicationCaseStatusId { get; set; }
        [SupplyParameterFromQuery]
        private string? ApplicationTypeId { get; set; }
        [SupplyParameterFromQuery]
        private string? ApplicationIntentId { get; set; }
        [SupplyParameterFromQuery]
        private string? ApplicationPriorityId { get; set; }
        [SupplyParameterFromQuery]
        private DateOnly? ReceivedDateFrom { get; set; }
        [SupplyParameterFromQuery]
        private DateOnly? ReceivedDateTo { get; set; }

        private List<Application>? SearchResults;

        private const int MaxResults = 1000;
        private bool IsBusy { get; set; }
        protected override async Task OnParametersSetAsync()
        {
            NavItems =
            [
                new BreadcrumbItem{ Text = localizer["Home Title"], Href = "" },
                new BreadcrumbItem{ Text = localizer["PPO Abbreviation"], Href = "PPO" },
                new BreadcrumbItem{ Text = localizer["Search Results Title"], IsCurrentPage = true },
            ];

            try
            {
                IsBusy = true;
                using var context = contextFactory.CreateDbContext();

                var query = context.PPOApplication.AsQueryable();


                if (ParishIds is not null && ParishIds.Length != 0)
                {
                    var parsedParishIds = ParishIds
                        .Where(id => int.TryParse(id, CultureInfo.InvariantCulture, out _))
                        .Select(id => int.Parse(id, CultureInfo.InvariantCulture))
                        .ToList();
                    if (parsedParishIds.Count != 0)
                    {
                        query = query.Where(d => d.PPOParishes.Any(p => parsedParishIds.Contains(p.ParishId)));
                    }

                }
                else if (ParishId is not null && int.TryParse(ParishId, CultureInfo.InvariantCulture, out int parsedParishId))
                {
                    query = query.Where(d => d.PPOParishes.Any(p => p.ParishId == parsedParishId));
                }
                if (ApplicationTypeId is not null && int.TryParse(ApplicationTypeId, CultureInfo.InvariantCulture, out int parsedApplicationTypeId))
                {
                    query = query.Where(d => d.ApplicationTypeId == parsedApplicationTypeId);
                }
                if (ApplicationCaseStatusId is not null && int.TryParse(ApplicationCaseStatusId, CultureInfo.InvariantCulture, out int parsedApplicationCaseStatusId))
                {
                    query = query.Where(d => d.CaseStatusId == parsedApplicationCaseStatusId);
                }
                if (ApplicationIntentId is not null && int.TryParse(ApplicationIntentId, CultureInfo.InvariantCulture, out int parsedApplicationIntentId))
                {
                    query = query.Where(d => d.PPOIntents.Any(p => parsedApplicationIntentId == p.IntentId));
                }
                if (ApplicationPriorityId is not null && int.TryParse(ApplicationPriorityId, CultureInfo.InvariantCulture, out int parsedApplicationPriorityId))
                {
                    query = query.Where(d => d.PriorityId == parsedApplicationPriorityId);
                }
                if (Location is not null)
                {
                    var place = await placesSearchService.GetPlaceByName(Location);
                    if(place is not null)
                    {
                        var bboxPolygon = new Polygon(
                            new LinearRing(
                                [
                                    new(decimal.ToDouble(place.MbrXMin), decimal.ToDouble(place.MbrYMin)),
                                    new(decimal.ToDouble(place.MbrXMin), decimal.ToDouble(place.MbrYMax)), 
                                    new(decimal.ToDouble(place.MbrXMax), decimal.ToDouble(place.MbrYMax)), 
                                    new(decimal.ToDouble(place.MbrXMin), decimal.ToDouble(place.MbrYMax)),
                                    new(decimal.ToDouble(place.MbrXMin), decimal.ToDouble(place.MbrYMin)),
                                ]
                            )
                        )
                        {
                            SRID = 27700,
                        };
                        query = query.Where(d => d.Geom.Intersects(bboxPolygon));
                    }
                }

                if (ReceivedDateFrom is not null)
                {
                    query = query.Where(d => d.ReceivedDate >= NodaTime.LocalDate.FromDateOnly(ReceivedDateFrom.Value));
                }
                if (ReceivedDateTo is not null)
                {
                    query = query.Where(d => d.ReceivedDate <= NodaTime.LocalDate.FromDateOnly(ReceivedDateTo.Value));
                }

                SearchResults = await query.OrderByDescending(d => d.Id).Take(MaxResults).ToListAsync();
            }catch(Exception ex)
            {
                logger.LogError(ex, "An error occurred rendering the applications list component");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
