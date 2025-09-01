using BlazorBootstrap;
using CSIDE.Web.Components.LandownerDeposits;
using CSIDE.Web.Components.Mapping;
using CSIDE.Data.Models.LandownerDeposits;
using CSIDE.Data.Services;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using NetTopologySuite.IO;
using NetTopologySuite.Geometries;

namespace CSIDE.Web.Components.Pages.LandownerDeposits
{
    public partial class Edit(ILandownerDepositService landownerDepositService, NavigationManager navigationManager, ILogger<Edit> logger, IRightsOfWayService rightsOfWayHelperService)
    {
        [Parameter]
        public int LandownerDepositId { get; set; }
        [Parameter]
        public int SecondaryId { get; set; }

        private LandownerDeposit? LandownerDeposit { get; set; }
        private ICollection<LandownerDepositTypeName> LandownerDepositTypeNames { get; set; } = [];
        private List<int> SelectedLandownerDepositTypes { get; set; } = [];
        private LandownerDepositEditForm? childLandownerDepositEditForm;

        private List<BreadcrumbItem>? NavItems;
        private Modal routeValidationModal = default!;
        private Modal errorModal = default!;

        private EditMap? editMap;
        private bool IsBusy { get; set; }
        private string? ErrorMessage { get; set; }
        private bool GeometryIsValid { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            NavItems =
            [
                new BreadcrumbItem{ Text = localizer["Home Title"], Href = "" },
                new BreadcrumbItem{ Text = localizer["Landowner Deposit Title"], Href="landowner-deposits" },
                new BreadcrumbItem{ Text = localizer["Landowner Deposit Details Title", $"{IDPrefixOptions.Value.LandownerDeposit}{LandownerDepositId}/{SecondaryId}"], Href=$"landowner-deposits/Details/{LandownerDepositId}/{SecondaryId}"},
                new BreadcrumbItem{ Text = localizer["Landowner Deposit Edit Title", $"{IDPrefixOptions.Value.LandownerDeposit}{LandownerDepositId}/{SecondaryId}"], IsCurrentPage = true },
            ];
            IsBusy = true;
            try
            {
                LandownerDepositTypeNames = await landownerDepositService.GetLandownerDepositTypeNameOptions();
                LandownerDeposit = await landownerDepositService.GetLandownerDepositById(LandownerDepositId, SecondaryId);
                SelectedLandownerDepositTypes = [.. LandownerDeposit!.LandownerDepositTypes.Select(t => t.LandownerDepositTypeNameId)];
                GeometryIsValid = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SubmitFormAsync()
        {
            if (LandownerDeposit is null || IsBusy)
            {
                ErrorMessage = null;
                return;
            }
            if (GeometryIsValid && await childLandownerDepositEditForm!.ValidateAsync())
            {
                IsBusy = true;
                StateHasChanged();
                try
                {
                    if (LandownerDeposit is not null)
                    {
                        await landownerDepositService.UpdateLandownerDeposit(LandownerDeposit, SelectedLandownerDepositTypes);

                        //redirect
                        NavigateBackToLandownerDepositDetailsPage();
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    ErrorMessage = localizer["Concurrency Error Message", localizer["Landowner Deposit Details Title", $"{IDPrefixOptions.Value.LandownerDeposit}{LandownerDeposit.Id}/{LandownerDeposit.SecondaryId}"]];
                    logger.LogWarning(ex, "A concurrency conflict occurred when updating a landowner deposit");
                }
                catch (Exception ex)
                {
                    ErrorMessage = localizer["Save Error Message"];
                    logger.LogError(ex, "Error saving landowner deposit");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private void NavigateBackToLandownerDepositDetailsPage()
        {
            navigationManager.NavigateTo($"landowner-deposits/Details/{LandownerDepositId}/{SecondaryId}");
        }

        private async Task ValidateGeometry(string features)
        {
            GeoJsonReader _geoJsonReader = new();
            FeatureCollection featureCollection = _geoJsonReader.Read<FeatureCollection>(features);

            CSIDE.Data.Validators.Geometry.GeometryValidator validator = new(localizer, rightsOfWayHelperService);

            var result = await validator.ValidateAsync(featureCollection, options => options.IncludeRuleSets("Polygon"));
            if (result.IsValid)
            {
                GeometryIsValid = true;
                if (LandownerDeposit is not null)
                {
                    Geometry geom = featureCollection[0].Geometry;
                    foreach (var feat in featureCollection.Skip(1))
                    {
                        geom = feat.Geometry.Union(geom);
                    }
                    if (geom.OgcGeometryType == OgcGeometryType.Polygon)
                    {
                        var geometryFactory = new GeometryFactory();
                        geom = geometryFactory.CreateMultiPolygon([(Polygon)geom]);
                    }
                    LandownerDeposit.Geom = (MultiPolygon)geom;
                    LandownerDeposit.Geom.SRID = 27700;
                }
            }
            else
            {
                GeometryIsValid = false;
            }
            StateHasChanged();
        }
        private async Task HideRouteValidationModal()
        {
            await routeValidationModal.HideAsync();
        }

        private async Task ClearDrawnGeometries()
        {
            if (LandownerDeposit is not null)
            {
                LandownerDeposit.Geom = MultiPolygon.Empty;
            }
            await editMap!.ClearDrawnGeometries();
            await routeValidationModal.HideAsync();
            await errorModal.HideAsync();
        }
    }
}
