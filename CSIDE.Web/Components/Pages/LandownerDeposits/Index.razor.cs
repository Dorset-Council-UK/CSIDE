using BlazorBootstrap;
using Blazored.FluentValidation;
using CSIDE.Data.Models.LandownerDeposits;
using CSIDE.Data.Models.Shared;
using CSIDE.Data.Services;
using CSIDE.Shared.Options;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace CSIDE.Web.Components.Pages.LandownerDeposits
{
    public partial class Index(
        ILandownerDepositService landownerDepositService,
        ISharedDataService sharedDataService,
        NavigationManager navigationManager,
        IOptions<CSIDEOptions> csideOptions)
    {
        private List<BreadcrumbItem>? NavItems;
        private LandownerDepositSearch? SearchParams;
        private string? LandownerDepositIDSearch;
        private IReadOnlyCollection<Parish>? Parishes { get; set; }

        private string? LandownerDepositIDSearchErrorMessage { get; set; }
        private FluentValidationValidator? _fluentValidationValidator;

        private bool UseMultiParishSelect { get; set; }
        private bool IsBusy { get; set; }
        private CSIDEOptions CsideOptions => csideOptions.Value;

        protected override async Task OnInitializedAsync()
        {
            NavItems =
            [
                new() { Text = localizer["Home Title"], Href = "" },
                new() { Text = localizer["Landowner Deposits Title"], IsCurrentPage = true },
            ];
            Parishes = await sharedDataService.GetParishes();
            SearchParams = new();
        }

        private async Task OnLandownerDepositIDSearchSubmit()
        {
            if (LandownerDepositIDSearch is not null)
            {
                IsBusy = true;
                LandownerDepositIDSearchErrorMessage = null;
                try
                {
                    if (!string.IsNullOrEmpty(IDPrefixOptions.Value.LandownerDeposit))
                    {
                        //remove any left in place prefixes
                        if (LandownerDepositIDSearch.StartsWith(IDPrefixOptions.Value.LandownerDeposit, StringComparison.OrdinalIgnoreCase))
                        {
                            LandownerDepositIDSearch = LandownerDepositIDSearch[IDPrefixOptions.Value.LandownerDeposit.Length..].Trim();
                        }
                    }
                    if (LandownerDepositIDSearch.Split("/").Length == 2 &&
                        int.TryParse(LandownerDepositIDSearch.Split("/")[0], CultureInfo.InvariantCulture, out int PrimaryId) &&
                        int.TryParse(LandownerDepositIDSearch.Split("/")[1], CultureInfo.InvariantCulture, out int SecondaryId)
                        )
                    {
                        var landownerDepositExists = await landownerDepositService.GetLandownerDepositById(PrimaryId, SecondaryId) is not null;
                        if (landownerDepositExists)
                        {
                            navigationManager.NavigateTo($"landowner-deposits/Details/{PrimaryId}/{SecondaryId}");
                            return;
                        }

                        LandownerDepositIDSearchErrorMessage = localizer["Landowner Deposit Not Found Error Message", LandownerDepositIDSearch];
                    }
                    else
                    {
                        LandownerDepositIDSearchErrorMessage = localizer["Landowner Deposit Not Found Error Message", LandownerDepositIDSearch];
                    }
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private async Task OnLandownerDepositSearchSubmit()
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
                        navigationManager.NavigateTo($"landowner-deposits/Items?{qs}");
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
