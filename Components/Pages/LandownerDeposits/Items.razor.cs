using BlazorBootstrap;
using CSIDE.Data;
using CSIDE.Data.Models.LandownerDeposits;
using CSIDE.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Globalization;

namespace CSIDE.Components.Pages.LandownerDeposits
{
    public partial class Items(IDbContextFactory<ApplicationDbContext> contextFactory, ILogger<Items> logger, IPlacesSearchService placesSearchService)
    {
        private List<BreadcrumbItem>? NavItems;

        [SupplyParameterFromQuery]
        private string[]? ParishIds { get; set; }
        [SupplyParameterFromQuery]
        private string? ParishId { get; set; }
        [SupplyParameterFromQuery]
        private string? Location { get; set; }

        private List<LandownerDeposit>? SearchResults;

        private const int MaxResults = 1000;
        private bool IsBusy { get; set; }
        protected override async Task OnParametersSetAsync()
        {
            NavItems =
            [
                new BreadcrumbItem{ Text = localizer["Home Title"], Href = "" },
                new BreadcrumbItem{ Text = localizer["Landowner Deposit Title"], Href="landowner-deposits" },
                new BreadcrumbItem{ Text = localizer["Search Results Title"], IsCurrentPage = true }
            ];
            try
            {
                IsBusy = true;
                using var context = contextFactory.CreateDbContext();

                var query = context.LandownerDeposits.AsQueryable();

                if (ParishIds is not null && ParishIds.Length != 0)
                {
                    var parsedParishIds = ParishIds
                        .Where(id => int.TryParse(id, CultureInfo.InvariantCulture, out _))
                        .Select(id => int.Parse(id, CultureInfo.InvariantCulture))
                        .ToList();
                    if (parsedParishIds.Count != 0)
                    {
                        query = query.Where(l => l.LandownerDepositParishes.Any(p => parsedParishIds.Contains(p.ParishId)));
                    }

                }
                else if (ParishId is not null && int.TryParse(ParishId, CultureInfo.InvariantCulture, out int parsedParishId))
                {
                    query = query.Where(d => d.LandownerDepositParishes.Any(p => p.ParishId == parsedParishId));
                }
                if (Location is not null)
                {
                    var place = await placesSearchService.GetPlaceByName(Location);
                    if (place is not null)
                    {
                        var bboxPolygon = new Polygon(
                            new LinearRing(
                                [
                                    new(decimal.ToDouble(place.MbrXMin), decimal.ToDouble(place.MbrYMin)),
                                    new(decimal.ToDouble(place.MbrXMin), decimal.ToDouble(place.MbrYMax)),
                                    new(decimal.ToDouble(place.MbrXMax), decimal.ToDouble(place.MbrYMax)),
                                    new(decimal.ToDouble(place.MbrXMin), decimal.ToDouble(place.MbrYMax)),
                                    new(decimal.ToDouble(place.MbrXMin), decimal.ToDouble(place.MbrYMin))
                                ]
                            )
                        )
                        {
                            SRID = 27700
                        };
                        query = query.Where(l => l.Geom.Intersects(bboxPolygon));
                    }
                }

                SearchResults = await query.OrderByDescending(l => l.ReceivedDate).Take(MaxResults).ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred rendering the landowner deposits list component");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
