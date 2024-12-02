using BlazorBootstrap;
using Blazored.FluentValidation;
using CSIDE.Data;
using CSIDE.Data.Models.Maintenance;
using CSIDE.Data.Models.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Web;

namespace CSIDE.Components.Pages.Maintenance
{
    public partial class Index(IDbContextFactory<ApplicationDbContext> contextFactory, NavigationManager navigationManager)
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

        private bool IsBusy { get; set; }

        protected override async Task OnInitializedAsync()
        {
            NavItems = new List<BreadcrumbItem>
        {
            new BreadcrumbItem{ Text = localizer["Home Title"], Href ="/" },
            new BreadcrumbItem{ Text = localizer["Maintenance Title"], IsCurrentPage = true }
        };
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
                    int JobIDSearchInt;
                    if (int.TryParse(JobIDSearch, out JobIDSearchInt))
                    {
                        using var context = contextFactory.CreateDbContext();
                        var jobExists = await context.MaintenanceJobs.AnyAsync(j => j.Id == JobIDSearchInt);
                        if (jobExists)
                        {
                            navigationManager.NavigateTo($"/Maintenance/Details/{JobIDSearchInt}");
                            return;
                        }
                        else
                        {
                            JobIDSearchErrorMessage = localizer["Maintenance Job Not Found Error Message", JobIDSearch];
                        }
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
                try
                {
                    if (await _fluentValidationValidator!.ValidateAsync())
                    {
                        var qs = GetQueryString(SearchParams);
                        navigationManager.NavigateTo($"/Maintenance/Jobs?{qs}");
                    }
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private string GetQueryString(object obj)
        {
            var result = new List<string>();
            var props = obj.GetType().GetProperties().Where(p => p.GetValue(obj, null) != null);
            foreach (var p in props)
            {
                var value = p.GetValue(obj, null);
                var enumerable = value as ICollection;
                if (enumerable != null)
                {
                    result.AddRange(from object v in enumerable select string.Format("{0}={1}", p.Name, HttpUtility.UrlEncode(v.ToString())));
                }
                else
                {
                    //TODO - Hacky way of forcing the date to convert into ISO date format
                    if (p.PropertyType == typeof(DateOnly?) && value is not null)
                    {
                        value = (value as DateOnly?)?.ToString("yyyy-MM-dd");
                    }
                    result.Add(string.Format("{0}={1}", p.Name, HttpUtility.UrlEncode(value?.ToString())));
                }
            }

            return string.Join("&", result.ToArray());
        }
    }
}
