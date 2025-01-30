using System.Globalization;
using BlazorBootstrap;
using CSIDE.Data;
using CSIDE.Data.Models.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace CSIDE.Components.Pages.Infrastructure
{
    public partial class Items(IDbContextFactory<ApplicationDbContext> contextFactory, ILogger<Items> logger)
    {
        private List<BreadcrumbItem>? NavItems;

        [SupplyParameterFromQuery]
        private string? RouteId { get; set; }
        [SupplyParameterFromQuery]
        private string[]? ParishIds { get; set; }
        [SupplyParameterFromQuery]
        private string? ParishId { get; set; }
        [SupplyParameterFromQuery]
        private string? MaintenanceTeamId { get; set; }
        [SupplyParameterFromQuery]
        private string? LoggedById { get; set; }
        [SupplyParameterFromQuery]
        private string? InfrastructureTypeId { get; set; }
        [SupplyParameterFromQuery]
        private DateOnly? InstallationDateFrom { get; set; }
        [SupplyParameterFromQuery]
        private DateOnly? InstallationDateTo { get; set; }

        private List<InfrastructureItem>? SearchResults;

        private const int MaxResults = 1000;
        private bool IsBusy { get; set; }
        protected override async Task OnParametersSetAsync()
        {
            NavItems =
            [
                new BreadcrumbItem{ Text = localizer["Home Title"], Href ="/" },
                new BreadcrumbItem{ Text = localizer["Infrastructure Title"], Href="Infrastructure" },
                new BreadcrumbItem{ Text = localizer["Search Results Title"], IsCurrentPage = true }
            ];
            try
            {
                IsBusy = true;
                using var context = contextFactory.CreateDbContext();

                var query = context.Infrastructure.AsQueryable();

                if (RouteId is not null)
                {
                    query = query.Where(i => i.RouteId == RouteId);
                }
                if (ParishIds is not null && ParishIds.Length != 0)
                {
                    var parsedParishIds = ParishIds
                        .Where(id => int.TryParse(id, CultureInfo.InvariantCulture, out _))
                        .Select(id => int.Parse(id, CultureInfo.InvariantCulture))
                        .ToList();
                    if (parsedParishIds.Count != 0)
                    {
                        query = query.Where(i => i.ParishId != null && parsedParishIds.Contains(i.ParishId.Value));
                    }
                }
                else if (ParishId is not null && int.TryParse(ParishId, CultureInfo.InvariantCulture, out int parsedParishId))
                {
                    query = query.Where(i => i.ParishId == parsedParishId);
                }
                if (MaintenanceTeamId is not null && int.TryParse(MaintenanceTeamId, CultureInfo.InvariantCulture, out int parsedAssignedToTeamId))
                {
                    query = query.Where(j => j.MaintenanceTeamId == parsedAssignedToTeamId);
                }
                if (InfrastructureTypeId is not null && int.TryParse(InfrastructureTypeId, CultureInfo.InvariantCulture, out int parsedStatusId))
                {
                    query = query.Where(i => i.InfrastructureTypeId == parsedStatusId);
                }
                if (InstallationDateFrom is not null)
                {
                    query = query.Where(i => i.InstallationDate >= LocalDate.FromDateOnly(InstallationDateFrom.Value));
                }
                if (InstallationDateTo is not null)
                {
                    query = query.Where(i => i.InstallationDate < LocalDate.FromDateOnly(InstallationDateTo.Value).PlusDays(1));
                }

                SearchResults = await query.OrderByDescending(i => i.InstallationDate).Take(MaxResults).ToListAsync();
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
