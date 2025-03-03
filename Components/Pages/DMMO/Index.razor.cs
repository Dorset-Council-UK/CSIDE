using BlazorBootstrap;
using Blazored.FluentValidation;
using CSIDE.Data;
using CSIDE.Data.Models.DMMO;
using CSIDE.Data.Models.Infrastructure;
using CSIDE.Data.Models.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CSIDE.Components.Pages.DMMO
{
    public partial class Index(IDbContextFactory<ApplicationDbContext> contextFactory, NavigationManager navigationManager)
    {
        private List<BreadcrumbItem>? NavItems;
        private DMMOSearch? SearchParams;
        private string? DMMOIDSearch;
        private ApplicationClaimedStatus[]? ClaimedStatuses;
        private ApplicationCaseStatus[]? CaseStatuses;
        private ApplicationType[]? ApplicationTypes;
        private Parish[]? Parishes { get; set; }

        private string? DMMOSearchErrorMessage { get; set; }
        private FluentValidationValidator? _fluentValidationValidator;
        private bool UseMultiParishSelect { get; set; }
        private bool IsBusy { get; set; }

        protected override async Task OnInitializedAsync()
        {
            NavItems =
           [
                new BreadcrumbItem{ Text = localizer["Home Title"], Href ="/" },
                new BreadcrumbItem{ Text = localizer["DMMO Abbreviation"], IsCurrentPage = true },
            ];

            using var context = contextFactory.CreateDbContext();
            Parishes = await context.Parishes.OrderBy(p => p.Name).ToArrayAsync();
            ClaimedStatuses = await context.DMMOApplicationClaimedStatuses.OrderBy(s => s.Name).ToArrayAsync();
            CaseStatuses = await context.DMMOApplicationCaseStatuses.OrderBy(s => s.Name).ToArrayAsync();
            ApplicationTypes = await context.DMMOApplicationTypes.OrderBy(t => t.Name).ToArrayAsync();

            SearchParams = new();
        }

        private async Task OnDMMOIDSearchSubmit()
        {
            if (DMMOIDSearch is not null)
            {
                IsBusy = true;
                DMMOSearchErrorMessage = null;
                try
                {
                    if (int.TryParse(DMMOIDSearch, CultureInfo.InvariantCulture, out int DMMOIDSearchInt))
                    {
                        using var context = contextFactory.CreateDbContext();
                        var dmmoExists = await context.DMMOApplication.AnyAsync(d => d.Id == DMMOIDSearchInt);
                        if (dmmoExists)
                        {
                            navigationManager.NavigateTo($"DMMO/Details/{DMMOIDSearchInt}");
                            return;
                        }

                        DMMOSearchErrorMessage = localizer["DMMO Not Found Error Message", DMMOIDSearch];
                    }
                    else
                    {
                        DMMOSearchErrorMessage = localizer["DMMO Not Found Error Message", DMMOIDSearch];
                    }
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private async Task OnDMMOSearchSubmit()
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
                        navigationManager.NavigateTo($"DMMO/Applications?{qs}");
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
