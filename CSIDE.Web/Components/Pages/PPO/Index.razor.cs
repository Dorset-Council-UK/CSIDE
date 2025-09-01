using BlazorBootstrap;
using Blazored.FluentValidation;
using CSIDE.Data.Models.PPO;
using CSIDE.Data.Models.Shared;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace CSIDE.Web.Components.Pages.PPO
{
    public partial class Index(IPPOService ppoService, ISharedDataService sharedDataService, NavigationManager navigationManager)
    {
        private List<BreadcrumbItem>? NavItems;
        private PPOSearch? SearchParams;
        private string? PPOIDSearch;
        private IReadOnlyCollection<ApplicationCaseStatus>? CaseStatuses = [];
        private IReadOnlyCollection<ApplicationIntent>? Intents = [];
        private IReadOnlyCollection<ApplicationPriority>? Priorities = [];
        private IReadOnlyCollection<ApplicationType>? ApplicationTypes = [];
        private IReadOnlyCollection<Parish> Parishes { get; set; } = [];

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

            Parishes = await sharedDataService.GetParishes();
            CaseStatuses = await ppoService.GetPPOCaseStatusOptions();
            Intents = await ppoService.GetPPOApplicationIntents();
            Priorities = await ppoService.GetPPOApplicationPriorities();
            ApplicationTypes = await ppoService.GetPPOApplicationTypeOptions();

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
                        var PPOExists = await ppoService.GetPPOApplicationById(PPOIDSearchInt) is not null;
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