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
    public partial class Create(IDbContextFactory<ApplicationDbContext> contextFactory, NavigationManager navigationManager, ILogger<Create> logger, IRightsOfWayHelperService rightsOfWayHelperService)
    {
        [Parameter]
        public int? ExistingDepositId { get; set; }
        [Parameter]
        public int? ExistingDepositSecondaryId { get; set; }
        private Modal routeValidationModal = default!;
        private Modal errorModal = default!;

        private CreateMap? createMap;
        private EditMap? editMap;

        private List<BreadcrumbItem>? NavItems;

        private LandownerDeposit? LandownerDeposit { get; set; }
        private LandownerDepositTypeName[]? LandownerDepositTypeNames { get; set; }

        private bool IsBusy { get; set; }
        private string? ErrorMessage { get; set; }
        private List<int> SelectedLandownerDepositTypes { get; set; } = [];
        private bool GeometryIsValid { get; set; }

        private LandownerDepositEditForm? childLandownerDepositEditForm;

        protected override async Task OnParametersSetAsync()
        {
            NavItems = new List<BreadcrumbItem>
        {
            new BreadcrumbItem{ Text = localizer["Home Title"], Href = "" },
            new BreadcrumbItem{ Text = localizer["Landowner Deposits Title"], Href="landowner-deposits" },
            new BreadcrumbItem{ Text = localizer["Landowner Deposit Create Title"], IsCurrentPage = true }
        };

            using var context = contextFactory.CreateDbContext();
            LandownerDepositTypeNames = await context.LandownerDepositTypeNames
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ToArrayAsync();
            LandownerDeposit = new()
            {
                Geom = null
            };
            if(ExistingDepositId != null && ExistingDepositSecondaryId != null)
            {
                //get the existing deposit
                var existingDeposit = await context.LandownerDeposits
                    .IgnoreAutoIncludes()
                    .FirstOrDefaultAsync(l => l.Id == ExistingDepositId && l.SecondaryId == ExistingDepositSecondaryId);
                if(existingDeposit is not null)
                {
                    LandownerDeposit.Id = existingDeposit.Id;
                    LandownerDeposit.Geom = existingDeposit.Geom;
                }
            }
            GeometryIsValid = true;
        }

        private async Task SubmitFormAsync()
        {
            if (IsBusy)
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

                        //TODO - get the highest secondary ID for this primary ID and increment it
                        var maxSecondaryId = await context.LandownerDeposits
                            .Where(ld => ld.Id == LandownerDeposit.Id)
                            .MaxAsync(ld => (int?)ld.SecondaryId) ?? 0;
                        LandownerDeposit.SecondaryId = maxSecondaryId + 1;
                        context.LandownerDeposits.Add(LandownerDeposit);

                        await context.SaveChangesAsync();
                        CreateLandownerDepositTypes(SelectedLandownerDepositTypes, LandownerDeposit.Id, LandownerDeposit.SecondaryId, context);
                        await context.SaveChangesAsync();
                        //redirect
                        navigationManager.NavigateTo($"landowner-deposits/Details/{LandownerDeposit.Id}/{LandownerDeposit.SecondaryId}");
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = localizer["Save Error Message"];
                    logger.LogError(ex, "An error occurred creating a landowner deposit");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private static void CreateLandownerDepositTypes(List<int> selectedLandownerDepositTypes, int LandownerDepositId, int LandownerDepositSecondaryId, ApplicationDbContext context)
        {
            //add new landowner deposit types
            foreach (int landownerDepositType in selectedLandownerDepositTypes)
            {
                context.LandownerDepositTypes.Add(new LandownerDepositType { LandownerDepositTypeNameId = landownerDepositType, LandownerDepositId = LandownerDepositId, LandownerDepositSecondaryId = LandownerDepositSecondaryId });
            }
            return;
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

        private async Task ClearDrawnGeometries()
        {
            if (LandownerDeposit is not null)
            {
                LandownerDeposit.Geom = null;
            }
            if(createMap != null)
            {
                await createMap.ClearDrawnGeometries();
            }
            if(editMap != null)
            {
                await editMap.ClearDrawnGeometries();
            }
            await routeValidationModal.HideAsync();
            await errorModal.HideAsync();
        }
    }
}

