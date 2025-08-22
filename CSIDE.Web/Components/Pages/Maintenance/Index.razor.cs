using BlazorBootstrap;
using Blazored.FluentValidation;
using CSIDE.Data;
using CSIDE.Data.Models.Maintenance;
using CSIDE.Data.Models.Shared;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CSIDE.Web.Components.Pages.Maintenance
{
    public partial class Index(IDbContextFactory<ApplicationDbContext> contextFactory, NavigationManager navigationManager, IMaintenanceJobsService maintenanceJobsService)
    {
        private List<BreadcrumbItem>? NavItems;
        private Search? SearchParams;
        private string? JobIDSearch;
        private JobStatus[]? JobStatuses { get; set; }
        private JobPriority[]? JobPriorities { get; set; }
        private Team[]? MaintenanceTeams { get; set; }
        private Parish[]? Parishes { get; set; }
        private string? JobIDSearchErrorMessage { get; set; }
        private FluentValidationValidator? _fluentValidationValidator;

        private bool UseMultiParishSelect { get; set; }
        private bool IsBusy { get; set; }

        protected override async Task OnInitializedAsync()
        {
            NavItems =
        [
            new() { Text = localizer["Home Title"], Href = "" },
            new() { Text = localizer["Maintenance Title"], IsCurrentPage = true },
        ];
            using var context = contextFactory.CreateDbContext();
            JobStatuses = await context.MaintenanceJobStatuses.OrderBy(s => s.SortOrder).ToArrayAsync();
            JobPriorities = await context.MaintenanceJobPriorities.OrderBy(p => p.SortOrder).ToArrayAsync();
            MaintenanceTeams = await context.MaintenanceTeams.OrderBy(p => p.Name).ToArrayAsync();
            Parishes = await context.Parishes.OrderBy(p => p.Name).ToArrayAsync();
            SearchParams = new();
        }

        private async Task OnJobIDSearchSubmit()
        {
            if (JobIDSearch is not null)
            {
                IsBusy = true;
                JobIDSearchErrorMessage = null;
                try
                {
                    if (int.TryParse(JobIDSearch, CultureInfo.InvariantCulture, out int JobIDSearchInt))
                    {
                        var jobExists = await maintenanceJobsService.GetMaintenanceJobById(JobIDSearchInt) is not null;
                        if (jobExists)
                        {
                            navigationManager.NavigateTo($"Maintenance/Details/{JobIDSearchInt}");
                            return;
                        }

                        JobIDSearchErrorMessage = localizer["Maintenance Job Not Found Error Message", JobIDSearch];
                    }
                    else
                    {
                        JobIDSearchErrorMessage = localizer["Maintenance Job Not Found Error Message", JobIDSearch];
                    }
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private async Task OnJobSearchSubmit()
        {
            if (SearchParams is not null)
            {
                IsBusy = true;
                if (UseMultiParishSelect)
                {
                    SearchParams.ParishId = null;
                }
                else
                {
                    SearchParams.ParishIds = [];
                }
                try
                {
                    if (await _fluentValidationValidator!.ValidateAsync())
                    {
                        var qs = Helpers.QueryStringHelper.GetQueryString(SearchParams);
                        navigationManager.NavigateTo($"Maintenance/Jobs?{qs}");
                    }
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }
    }
}
