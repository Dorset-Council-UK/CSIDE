using BlazorBootstrap;
using CSIDE.Components.LandownerDeposits;
using CSIDE.Components.Mapping;
using CSIDE.Data;
using CSIDE.Data.Models.LandownerDeposits;
using CSIDE.Services;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using NetTopologySuite.IO;
using NetTopologySuite.Geometries;

namespace CSIDE.Components.Pages.LandownerDeposits
{
    public partial class Edit(IDbContextFactory<ApplicationDbContext> contextFactory, NavigationManager navigationManager, ILogger<Edit> logger, IRightsOfWayHelperService rightsOfWayHelperService)
    {
        [Parameter]
        public int LandownerDepositId { get; set; }

        private LandownerDeposit? LandownerDeposit { get; set; }
        private LandownerDepositTypeName[]? LandownerDepositTypeNames { get; set; }
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
                new BreadcrumbItem{ Text = localizer["Landowner Deposit Details Title", $"{IDPrefixOptions.Value.LandownerDeposit}{LandownerDepositId}"], Href=$"landowner-deposits/Details/{LandownerDepositId}"},
                new BreadcrumbItem{ Text = localizer["Landowner Deposit Edit Title", $"{IDPrefixOptions.Value.LandownerDeposit}{LandownerDepositId}"], IsCurrentPage = true }
            ];
            IsBusy = true;
            try
            {
                using var context = contextFactory.CreateDbContext();
                LandownerDepositTypeNames = await context.LandownerDepositTypeNames
                    .AsNoTracking()
                    .OrderBy(p => p.Name)
                    .ToArrayAsync();
                LandownerDeposit = await context.LandownerDeposits
                    .IgnoreAutoIncludes()
                    .Include(l => l.LandownerDepositTypes)
                    .Include(l => l.LandownerDepositParishes)
                    .FirstOrDefaultAsync(i => i.Id == LandownerDepositId);
                SelectedLandownerDepositTypes = LandownerDeposit!.LandownerDepositTypes
                    .Select(t => t.LandownerDepositTypeNameId)
                    .ToList();
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
                        using var context = contextFactory.CreateDbContext();


                        //get the existing job to enable the smarter change tracker.
                        //Without this, all properties are identified as tracked, since
                        //the DbContext is different from when the entity was queried
                        var existingDeposit = await context.LandownerDeposits.FindAsync(LandownerDeposit.Id) ?? throw new Exception($"Landowner Deposit being edited (ID: {LandownerDeposit.Id}) was not found prior to updating");

                        // Store the original version for concurrency checking
                        uint originalVersion = LandownerDeposit.Version;
                        // Update values
                        context.Entry(existingDeposit).CurrentValues.SetValues(LandownerDeposit);
                        // Restore original version to ensure concurrency check works
                        context.Entry(existingDeposit).Property(j => j.Version).OriginalValue = originalVersion;

                        await UpdateLandownerDepositTypes(SelectedLandownerDepositTypes, context);
                        await context.SaveChangesAsync();
                        //redirect
                        NavigateBackToLandownerDepositDetailsPage();
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    ErrorMessage = localizer["Concurrency Error Message", localizer["Landowner Deposit Details Title", $"{IDPrefixOptions.Value.LandownerDeposit}{LandownerDeposit.Id}"]];
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

        private async Task UpdateLandownerDepositTypes(List<int> selectedLandownerDepositTypes, ApplicationDbContext context)
        {
            if (LandownerDeposit is null) return;
            if (selectedLandownerDepositTypes == null)
            {
                await context.LandownerDepositTypes
                    .Where(c => c.LandownerDepositId == LandownerDeposit.Id)
                    .ExecuteDeleteAsync();
                return;
            }

            //delete types not needed anymore
            await context.LandownerDepositTypes
                .Where(c => c.LandownerDepositId == LandownerDepositId && !selectedLandownerDepositTypes
                .Contains(c.LandownerDepositTypeNameId))
                .ExecuteDeleteAsync();

            //add new landowner deposit types
            foreach (int landownerDepositType in selectedLandownerDepositTypes)
            {
                if (!context.LandownerDepositTypes.Where(c => c.LandownerDepositId == LandownerDeposit.Id && c.LandownerDepositTypeNameId == landownerDepositType).Any())
                {
                    context.LandownerDepositTypes.Add(new LandownerDepositType { LandownerDepositTypeNameId = landownerDepositType, LandownerDepositId = LandownerDeposit.Id });
                }
            }
            return;
        }

        private void NavigateBackToLandownerDepositDetailsPage()
        {
            navigationManager.NavigateTo($"landowner-deposits/Details/{LandownerDepositId}");
        }

        private async Task ValidateGeometry(string features)
        {
            GeoJsonReader _geoJsonReader = new();
            FeatureCollection featureCollection = _geoJsonReader.Read<FeatureCollection>(features);

            CSIDE.Validators.Geometry.GeometryValidator validator = new(contextFactory, localizer, rightsOfWayHelperService);

            var result = await validator.ValidateAsync(featureCollection, options => options.IncludeRuleSets("Polygon"));
            if (result.IsValid)
            {
                GeometryIsValid = true;
                if (LandownerDeposit is not null)
                {
                    Geometry geom = featureCollection.First().Geometry;
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
                LandownerDeposit.Geom = null;
            }
            await editMap!.ClearDrawnGeometries();
            await routeValidationModal.HideAsync();
            await errorModal.HideAsync();
        }
    }
}
