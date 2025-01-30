using BlazorBootstrap;
using CSIDE.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CSIDE.Components.Pages.RightsOfWay
{
    public partial class List(IDbContextFactory<ApplicationDbContext> contextFactory, ILogger<List> logger)
    {
        private List<BreadcrumbItem>? NavItems;

        [SupplyParameterFromQuery]
        private string? RouteId { get; set; }
        [SupplyParameterFromQuery]
        private string[]? ParishIds { get; set; }
        [SupplyParameterFromQuery]
        private string? ParishId { get; set; }
        [SupplyParameterFromQuery]
        private string? Name { get; set; }
        [SupplyParameterFromQuery]
        private string? MaintenanceTeamId { get; set; }
        [SupplyParameterFromQuery]
        private string? OperationalStatusId { get; set; }
        [SupplyParameterFromQuery]
        private string? RouteTypeId { get; set; }

        private List<Data.Models.RightsOfWay.Route>? SearchResults;

        private const int MaxResults = 1000;
        private bool IsBusy { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            NavItems =
            [
                new BreadcrumbItem{ Text = localizer["Home Title"], Href ="/" },
                new BreadcrumbItem{ Text = localizer["Rights of Way Title"], Href="rights-of-way" },
                new BreadcrumbItem{ Text = localizer["Search Results Title"], IsCurrentPage = true }
            ];
            try {
                IsBusy = true;
                using var context = contextFactory.CreateDbContext();

                var query = context.Routes.AsQueryable();

                if (RouteId is not null)
                {
                    query = query.Where(j => j.RouteCode == RouteId);
                }
                if (Name is not null)
                {
                    query = query.Where(j => EF.Functions.ILike(j.Name!, $"%{Name}%"));
                }
                if (ParishIds is not null && ParishIds.Length != 0)
                {
                    var parsedParishIds = ParishIds
                        .Where(id => int.TryParse(id, CultureInfo.InvariantCulture, out _))
                        .Select(id => int.Parse(id, CultureInfo.InvariantCulture))
                        .ToList();
                    if (parsedParishIds.Count != 0)
                    {
                        query = query.Where(j => j.ParishId != null && parsedParishIds.Contains(j.ParishId.Value));
                    }
                }else if(ParishId is not null && int.TryParse(ParishId, CultureInfo.InvariantCulture, out int parsedParishId))
                {
                    query = query.Where(j => j.ParishId == parsedParishId);
                }
                if (MaintenanceTeamId is not null && int.TryParse(MaintenanceTeamId, CultureInfo.InvariantCulture, out int parsedMaintenanceTeamId))
                {
                    query = query.Where(j => j.MaintenanceTeamId == parsedMaintenanceTeamId);
                }
                if (OperationalStatusId is not null && int.TryParse(OperationalStatusId, CultureInfo.InvariantCulture, out int parsedOperationalStatusId))
                {
                    query = query.Where(j => j.OperationalStatusId == parsedOperationalStatusId);
                }
                if (RouteTypeId is not null && int.TryParse(RouteTypeId, CultureInfo.InvariantCulture, out int parsedRouteTypeId))
                {
                    query = query.Where(j => j.RouteTypeId == parsedRouteTypeId);
                }

                SearchResults = await query.OrderByDescending(r => r.RouteCode).Take(MaxResults).ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred rendering the jobs list component");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
