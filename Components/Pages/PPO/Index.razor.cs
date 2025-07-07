using BlazorBootstrap;
using Blazored.FluentValidation;
using CSIDE.Data;
using CSIDE.Data.Models.PPO;
using CSIDE.Data.Models.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CSIDE.Components.Pages.PPO
{
    public partial class Index(IDbContextFactory<ApplicationDbContext> contextFactory, NavigationManager navigationManager)
    {

        private List<BreadcrumbItem>? NavItems;
        private PPOSearch? SearchParams;
        private string? PPOIDSearch;
        private ApplicationCaseStatus[]? CaseStatuses;
        private ApplicationIntent[]? Intents;
        private ApplicationPriority[]? Priorities;
        private ApplicationType[]? ApplicationTypes;
        private Parish[]? Parishes { get; set; }

        private string? PPOSearchErrorMessage { get; set; }
        private FluentValidationValidator? _fluentValidationValidator;
        private bool UseMultiParishSelect { get; set; }
        private bool IsBusy { get; set; }

        protected override async Task OnInitializedAsync()
        {
            NavItems =
           [
                new BreadcrumbItem{ Text = localizer["Home Title"], Href = "" },
                new BreadcrumbItem{ Text = localizer["PPO Abbreviation"], IsCurrentPage = true },
            ];

            using var context = contextFactory.CreateDbContext();
            Parishes = await context.Parishes.OrderBy(p => p.Name).ToArrayAsync();
            CaseStatuses = await context.PPOApplicationCaseStatuses.OrderBy(s => s.Name).ToArrayAsync();
            Intents = await context.PPOApplicationIntents.OrderBy(i => i.Name).ToArrayAsync();
            Priorities = await context.PPOApplicationPriorities.OrderBy(p => p.SortOrder).ToArrayAsync();
            ApplicationTypes = await context.PPOApplicationTypes.OrderBy(t => t.Name).ToArrayAsync();

            SearchParams = new();
        }

        private async Task OnPPOIDSearchSubmit()
        {
            if (PPOIDSearch is not null)
            {
                IsBusy = true;
                PPOSearchErrorMessage = null;
                try
                {
                    if (int.TryParse(PPOIDSearch, CultureInfo.InvariantCulture, out int PPOIDSearchInt))
                    {
                        using var context = contextFactory.CreateDbContext();
                        var PPOExists = await context.PPOApplication.AnyAsync(d => d.Id == PPOIDSearchInt);
                        if (PPOExists)
                        {
                            navigationManager.NavigateTo($"PPO/Details/{PPOIDSearchInt}");
                            return;
                        }

                        PPOSearchErrorMessage = localizer["PPO Not Found Error Message", PPOIDSearch];
                    }
                    else
                    {
                        PPOSearchErrorMessage = localizer["PPO Not Found Error Message", PPOIDSearch];
                    }
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private async Task OnPPOSearchSubmit()
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
                        navigationManager.NavigateTo($"PPO/Applications?{qs}");
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