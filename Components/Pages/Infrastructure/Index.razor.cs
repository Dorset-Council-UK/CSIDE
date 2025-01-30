using System.Web;
using BlazorBootstrap;
using Blazored.FluentValidation;
using CSIDE.Data;
using CSIDE.Data.Models.Infrastructure;
using CSIDE.Data.Models.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using CSIDE.Data.Models.Maintenance;
using System.Globalization;

namespace CSIDE.Components.Pages.Infrastructure
{
    public partial class Index(IDbContextFactory<ApplicationDbContext> contextFactory, NavigationManager navigationManager)
    {
        private List<BreadcrumbItem>? NavItems;
        private InfrastructureSearch? SearchParams;
        private string? InfrastructureIDSearch;
        private InfrastructureType[]? InfrastructureTypes;
        private Parish[]? Parishes { get; set; }
        private Team[]? MaintenanceTeams { get; set; }

        private string? InfrastructureIDSearchErrorMessage { get; set; }
        private FluentValidationValidator? _fluentValidationValidator;

        private bool UseMultiParishSelect { get; set; }
        private bool IsBusy { get; set; }


        protected override async Task OnInitializedAsync()
        {
            NavItems = new List<BreadcrumbItem>
        {
            new BreadcrumbItem{ Text = localizer["Home Title"], Href ="/" },
            new BreadcrumbItem{ Text = localizer["Infrastructure Title"], IsCurrentPage = true }
        };
            using var context = contextFactory.CreateDbContext();
            Parishes = await context.Parishes.OrderBy(p => p.Name).ToArrayAsync();
            InfrastructureTypes = await context.InfrastructureTypes.OrderBy(t => t.Name).ToArrayAsync();
            MaintenanceTeams = await context.MaintenanceTeams.OrderBy(p => p.Name).ToArrayAsync();
            SearchParams = new();
        }
        private async Task OnInfrastructureIDSearchSubmit()
        {
            if (InfrastructureIDSearch is not null)
            {
                IsBusy = true;
                InfrastructureIDSearchErrorMessage = null;
                try
                {
                    if (int.TryParse(InfrastructureIDSearch, CultureInfo.InvariantCulture, out int InfrastructureIDSearchInt))
                    {
                        using var context = contextFactory.CreateDbContext();
                        var infrastructureExists = await context.Infrastructure.AnyAsync(j => j.Id == InfrastructureIDSearchInt);
                        if (infrastructureExists)
                        {
                            navigationManager.NavigateTo($"Infrastructure/Details/{InfrastructureIDSearchInt}");
                            return;
                        }
                        else
                        {
                            InfrastructureIDSearchErrorMessage = localizer["Infrastructure Not Found Error Message", InfrastructureIDSearch];
                        }
                    }
                    else
                    {
                        InfrastructureIDSearchErrorMessage = localizer["Infrastructure Not Found Error Message", InfrastructureIDSearch];
                    }
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private async Task OnInfrastructureSearchSubmit()
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
                        navigationManager.NavigateTo($"Infrastructure/Items?{qs}");
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
