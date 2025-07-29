using BlazorBootstrap;
using Blazored.FluentValidation;
using CSIDE.Data;
using CSIDE.Data.Models.LandownerDeposits;
using CSIDE.Data.Models.Shared;
using CSIDE.Options;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace CSIDE.Components.Pages.LandownerDeposits
{
    public partial class Index(IDbContextFactory<ApplicationDbContext> contextFactory, NavigationManager navigationManager, IOptions<CSIDEOptions> CsideOptions)
    {
        private List<BreadcrumbItem>? NavItems;
        private LandownerDepositSearch? SearchParams;
        private string? LandownerDepositIDSearch;
        private Parish[]? Parishes { get; set; }

        private string? LandownerDepositIDSearchErrorMessage { get; set; }
        private FluentValidationValidator? _fluentValidationValidator;

        private bool UseMultiParishSelect { get; set; }
        private bool IsBusy { get; set; }
        private CSIDEOptions _csideOptions => CsideOptions.Value;

        protected override async Task OnInitializedAsync()
        {
            NavItems = new List<BreadcrumbItem>
            {
                new BreadcrumbItem{ Text = localizer["Home Title"], Href = "" },
                new BreadcrumbItem{ Text = localizer["Landowner Deposits Title"], IsCurrentPage = true }
            };
            using var context = contextFactory.CreateDbContext();
            Parishes = await context.Parishes.OrderBy(p => p.Name).ToArrayAsync();
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
                    if (LandownerDepositIDSearch.Split("/").Length == 2 &&
                        int.TryParse(LandownerDepositIDSearch.Split("/")[0], CultureInfo.InvariantCulture, out int PrimaryId) &&
                        int.TryParse(LandownerDepositIDSearch.Split("/")[1], CultureInfo.InvariantCulture, out int SecondaryId)
                        )
                    {
                        using var context = contextFactory.CreateDbContext();
                        var landownerDepositExists = await context.LandownerDeposits.AnyAsync(l => l.Id == PrimaryId && l.SecondaryId == SecondaryId);
                        if (landownerDepositExists)
                        {
                            navigationManager.NavigateTo($"landowner-deposits/Details/{PrimaryId}/{SecondaryId}");
                            return;
                        }
                        else
                        {
                            LandownerDepositIDSearchErrorMessage = localizer["Landowner Deposit Not Found Error Message", LandownerDepositIDSearch];
                        }
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
