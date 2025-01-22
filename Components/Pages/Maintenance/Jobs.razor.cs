using BlazorBootstrap;
using CSIDE.Data;
using CSIDE.Data.Models.Maintenance;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using System.Globalization;

namespace CSIDE.Components.Pages.Maintenance
{
    public partial class Jobs(IDbContextFactory<ApplicationDbContext> contextFactory, ILogger<Jobs> logger)
    {
        private List<BreadcrumbItem>? NavItems;

        [SupplyParameterFromQuery]
        private string? RouteId { get; set; }
        [SupplyParameterFromQuery]
        private string[]? ParishIds { get; set; }
        [SupplyParameterFromQuery]
        private string? ParishId { get; set; }
        [SupplyParameterFromQuery]
        private string? LoggedById { get; set; }
        [SupplyParameterFromQuery]
        private string? AssignedToTeamId { get; set; }
        [SupplyParameterFromQuery]
        private string? JobPriorityId { get; set; }
        [SupplyParameterFromQuery]
        private string? JobStatusId { get; set; }
        [SupplyParameterFromQuery]
        private DateOnly? LogDateFrom { get; set; }
        [SupplyParameterFromQuery]
        private DateOnly? LogDateTo { get; set; }
        [SupplyParameterFromQuery]
        private DateOnly? CompletedDateFrom { get; set; }
        [SupplyParameterFromQuery]
        private DateOnly? CompletedDateTo { get; set; }

        private List<Job>? SearchResults;

        protected override async Task OnParametersSetAsync()
        {
            NavItems =
            [
                new BreadcrumbItem{ Text = localizer["Home Title"], Href ="/" },
                new BreadcrumbItem{ Text = localizer["Maintenance Title"], Href="Maintenance" },
                new BreadcrumbItem{ Text = localizer["Search Results Title"], IsCurrentPage = true }
            ];
            try
            {
                using var context = contextFactory.CreateDbContext();

                var query = context.MaintenanceJobs.AsQueryable();

                if (RouteId is not null)
                {
                    query = query.Where(j => j.RouteId == RouteId);
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

                }
                else if (ParishId is not null && int.TryParse(ParishId, CultureInfo.InvariantCulture, out int parsedParishId))
                {
                    query = query.Where(j => j.ParishId == parsedParishId);
                }
                if (AssignedToTeamId is not null && int.TryParse(AssignedToTeamId, CultureInfo.InvariantCulture, out int parsedAssignedToTeamId))
                {
                    query = query.Where(j => j.MaintenanceTeamId == parsedAssignedToTeamId);
                }
                if (JobPriorityId is not null && int.TryParse(JobPriorityId, CultureInfo.InvariantCulture, out int parsedPriorityId))
                {
                    query = query.Where(j => j.JobPriorityId == parsedPriorityId);
                }
                if (JobStatusId is not null && int.TryParse(JobStatusId, CultureInfo.InvariantCulture, out int parsedStatusId))
                {
                    query = query.Where(j => j.JobStatusId == parsedStatusId);
                }
                if (LogDateFrom is not null)
                {
                    query = query.Where(j => j.LogDate >= ConvertDateToInstant(LogDateFrom.Value));
                }
                if (LogDateTo is not null)
                {
                    query = query.Where(j => j.LogDate < ConvertDateToInstant(LogDateTo.Value).Plus(Duration.FromDays(1)));
                }
                if (CompletedDateFrom is not null)
                {
                    query = query.Where(j => j.CompletionDate >= NodaTime.LocalDate.FromDateOnly(CompletedDateFrom.Value));
                }

                if (CompletedDateTo is not null)
                {
                    query = query.Where(j => j.CompletionDate < NodaTime.LocalDate.FromDateOnly(CompletedDateTo.Value).PlusDays(1));
                }

                SearchResults = await query.OrderByDescending(j => j.LogDate).Take(500).ToListAsync();
            }catch(Exception ex)
            {
                logger.LogError(ex, "An error occurred rendering the jobs list component");
            }
        }

        private static Instant ConvertDateToInstant(DateOnly date)
        {
            var timezone = NodaTime.DateTimeZoneProviders.Tzdb.GetSystemDefault();
            var localDate = NodaTime.LocalDate.FromDateOnly(date);
            var zonedDate = localDate.AtStartOfDayInZone(timezone);
            return zonedDate.ToInstant();
        }
    }
}
