using BlazorBootstrap;
using Blazored.FluentValidation;
using CSIDE.Data.Models.Maintenance;
using CSIDE.Data.Models.Shared;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace CSIDE.Web.Components.Pages.Maintenance
{
    public partial class Index(NavigationManager navigationManager, IMaintenanceJobsService maintenanceJobsService, ISharedDataService sharedDataService)
    {
        private List<BreadcrumbItem>? NavItems;
        private Search? SearchParams;
        private string? JobIDSearch;
        private JobStatus[]? JobStatuses { get; set; }
        private JobPriority[]? JobPriorities { get; set; }
        private IReadOnlyCollection<Team> MaintenanceTeams { get; set; } = [];
        private IReadOnlyCollection<Parish>? Parishes { get; set; }
        private ProblemType[]? ProblemTypes { get; set; }

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
            JobStatuses = await maintenanceJobsService.GetMaintenanceJobStatuses();
            JobPriorities = await maintenanceJobsService.GetMaintenanceJobPriorities();
            MaintenanceTeams = await maintenanceJobsService.GetMaintenanceTeams();
            Parishes = await sharedDataService.GetParishes();
            ProblemTypes = await maintenanceJobsService.GetMaintenanceProblemTypes();
            SearchParams = new();
        }

        private async Task OnJobIDSearchSubmit()
        {
            if (JobIDSearch is not null)
            {
                IsBusy = true;
                JobIDSearchErrorMessage = null;
                if (!string.IsNullOrEmpty(IDPrefixOptions.Value.Maintenance) &&
                    JobIDSearch.StartsWith(IDPrefixOptions.Value.Maintenance, StringComparison.OrdinalIgnoreCase))
                {
                    //remove any left in place prefixes
                    JobIDSearch = JobIDSearch[IDPrefixOptions.Value.Maintenance.Length..].Trim();
                }
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

        private void ProblemTypeChanged(ProblemType ProblemType, ChangeEventArgs eventArgs)
        {
            if (SearchParams is null)
            {
                return;
            }

            var problemTypeId = ProblemType.Id.ToString(CultureInfo.InvariantCulture);
            var selectedProblemTypeIds = SearchParams.ProblemTypeIds ?? [];

            if (Convert.ToBoolean(eventArgs.Value))
            {
                if (Array.IndexOf(selectedProblemTypeIds, problemTypeId) < 0)
                {
                    SearchParams.ProblemTypeIds = [.. selectedProblemTypeIds, problemTypeId];
                }
            }
            else
            {
                if (Array.IndexOf(selectedProblemTypeIds, problemTypeId) >= 0)
                {
                    SearchParams.ProblemTypeIds = [.. selectedProblemTypeIds.Where(id => !string.Equals(id, problemTypeId, StringComparison.Ordinal))];
                }
            }
        }
    }
}
