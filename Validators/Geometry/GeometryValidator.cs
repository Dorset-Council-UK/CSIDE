using CSIDE.Data;
using CSIDE.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace CSIDE.Validators.Geometry
{
    public class GeometryValidator : AbstractValidator<FeatureCollection>
    {
        readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        readonly IStringLocalizer<Properties.Resources> _localizer;
        readonly IRightsOfWayHelperService _geometryValidationService;
        public GeometryValidator(IDbContextFactory<ApplicationDbContext> contextFactory, IStringLocalizer<Properties.Resources> localizer, IRightsOfWayHelperService geometryValidationService)
        {
            _localizer = localizer;
            _contextFactory = contextFactory;
            _geometryValidationService = geometryValidationService;

            RuleSet("Single Point", () =>
            {

                RuleFor(g => g.Count).Equal(1).WithMessage(localizer["Invalid Geometry Validation Message"]).WithErrorCode("INVALID_GEOM");

                RuleFor(g => g.First().Geometry.OgcGeometryType).Equal(NetTopologySuite.Geometries.OgcGeometryType.Point).WithMessage(localizer["Invalid Geometry Validation Message"]).WithErrorCode("INVALID_GEOM");
                RuleFor(f => f.First().Geometry.Centroid.X).GreaterThanOrEqualTo(0).LessThanOrEqualTo(700_000).WithMessage(localizer["Geometry Outside UK Validation Message"]).WithErrorCode("GEOM_OUTSIDE_BOUNDS");
                RuleFor(f => f.First().Geometry.Centroid.Y).GreaterThanOrEqualTo(0).LessThanOrEqualTo(1_300_000).WithMessage(localizer["Geometry Outside UK Validation Message"]).WithErrorCode("GEOM_OUTSIDE_BOUNDS");
            });

            RuleSet("Multi Point", () =>
            {
                RuleFor(g => g.Count).GreaterThanOrEqualTo(1).WithMessage(localizer["Invalid Geometry Validation Message"]).WithErrorCode("INVALID_GEOM");

                RuleFor(g => g.First().BoundingBox!.MinX).GreaterThanOrEqualTo(0).WithMessage(localizer["Geometry Outside UK Validation Message"]).WithErrorCode("GEOM_OUTSIDE_BOUNDS");
                RuleFor(g => g.First().BoundingBox!.MinY).GreaterThanOrEqualTo(0).WithMessage(localizer["Geometry Outside UK Validation Message"]).WithErrorCode("GEOM_OUTSIDE_BOUNDS");
                RuleFor(g => g.First().BoundingBox!.MaxX).LessThanOrEqualTo(700_000).WithMessage(localizer["Geometry Outside UK Validation Message"]).WithErrorCode("GEOM_OUTSIDE_BOUNDS");
                RuleFor(g => g.First().BoundingBox!.MaxY).LessThanOrEqualTo(1_300_000).WithMessage(localizer["Geometry Outside UK Validation Message"]).WithErrorCode("GEOM_OUTSIDE_BOUNDS");
            });

            RuleSet("Point On Route", () =>
            {
                RuleFor(g => g).MustAsync(PointOnRoute).WithErrorCode("NO_ROUTE_NEARBY");
            });

            RuleSet("Line String", () =>
            {
                //Add rule to check that the featurecollection contains only linestring or multiline strings
                RuleFor(g => g).Must(g => g.All(f => f.Geometry.OgcGeometryType == OgcGeometryType.LineString || f.Geometry.OgcGeometryType == OgcGeometryType.MultiLineString))
                    .WithMessage(localizer["Invalid Geometry Validation Message"])
                    .WithErrorCode("INVALID_GEOM");

                RuleFor(g => g).Must(g => g.All(f => f.Geometry.Envelope.Coordinates.First().X >= 0))
                    .WithMessage(localizer["Geometry Outside UK Validation Message"])
                    .WithErrorCode("GEOM_OUTSIDE_BOUNDS");

                RuleFor(g => g).Must(g => g.All(f => f.Geometry.Envelope.Coordinates.First().Y >= 0))
                    .WithMessage(localizer["Geometry Outside UK Validation Message"])
                    .WithErrorCode("GEOM_OUTSIDE_BOUNDS");

                RuleFor(g => g).Must(g => g.All(f => f.Geometry.Envelope.Coordinates[2].X <= 700_000))
                    .WithMessage(localizer["Geometry Outside UK Validation Message"])
                    .WithErrorCode("GEOM_OUTSIDE_BOUNDS");

                RuleFor(g => g).Must(g => g.All(f => f.Geometry.Envelope.Coordinates[2].Y <= 1_300_000))
                    .WithMessage(localizer["Geometry Outside UK Validation Message"])
                    .WithErrorCode("GEOM_OUTSIDE_BOUNDS");

                RuleFor(g => g.Count).GreaterThanOrEqualTo(1).WithMessage(localizer["Invalid Geometry Validation Message"]).WithErrorCode("INVALID_GEOM");
            });

            RuleSet("Polygon", () =>
            {
                //Add rule to check that the featurecollection contains only polygon or multipolygons
                RuleFor(g => g).Must(g => g.All(f => f.Geometry.OgcGeometryType == OgcGeometryType.Polygon || f.Geometry.OgcGeometryType == OgcGeometryType.MultiPolygon))
                    .WithMessage(localizer["Invalid Geometry Validation Message"])
                    .WithErrorCode("INVALID_GEOM");

                RuleFor(g => g).Must(g => g.All(f => f.Geometry.Envelope.Coordinates.First().X >= 0))
                     .WithMessage(localizer["Geometry Outside UK Validation Message"])
                     .WithErrorCode("GEOM_OUTSIDE_BOUNDS");

                RuleFor(g => g).Must(g => g.All(f => f.Geometry.Envelope.Coordinates.First().Y >= 0))
                    .WithMessage(localizer["Geometry Outside UK Validation Message"])
                    .WithErrorCode("GEOM_OUTSIDE_BOUNDS");

                RuleFor(g => g).Must(g => g.All(f => f.Geometry.Envelope.Coordinates[2].X <= 700_000))
                    .WithMessage(localizer["Geometry Outside UK Validation Message"])
                    .WithErrorCode("GEOM_OUTSIDE_BOUNDS");

                RuleFor(g => g).Must(g => g.All(f => f.Geometry.Envelope.Coordinates[2].Y <= 1_300_000))
                    .WithMessage(localizer["Geometry Outside UK Validation Message"])
                    .WithErrorCode("GEOM_OUTSIDE_BOUNDS");

                RuleFor(g => g.Count).GreaterThanOrEqualTo(1).WithMessage(localizer["Invalid Geometry Validation Message"]).WithErrorCode("INVALID_GEOM");
            });
        }

        private async Task<bool> PointOnRoute(FeatureCollection features, CancellationToken token)
        {
            var point = features.First().Geometry.Centroid;
            point.SRID = 27700;
            
            var NearestRoute = await _geometryValidationService.GetNearestRouteAsync(point);

            return (NearestRoute is not null);
        }

    }
}
