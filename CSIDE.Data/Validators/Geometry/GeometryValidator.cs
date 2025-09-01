using CSIDE.Data.Services;
using FluentValidation;
using Microsoft.Extensions.Localization;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace CSIDE.Data.Validators.Geometry
{
    public class GeometryValidator : AbstractValidator<FeatureCollection>
    {
        readonly IStringLocalizer<CSIDE.Shared.Properties.Resources> _localizer;
        readonly IRightsOfWayService _geometryValidationService;
        public GeometryValidator(IStringLocalizer<CSIDE.Shared.Properties.Resources> localizer, IRightsOfWayService geometryValidationService)
        {
            _localizer = localizer;
            _geometryValidationService = geometryValidationService;

            RuleSet("Single Point", () =>
            {

                RuleFor(g => g.Count).Equal(1).WithMessage(localizer["Invalid Geometry Validation Message"]).WithErrorCode("INVALID_GEOM");

                RuleFor(g => g[0].Geometry.OgcGeometryType).Equal(NetTopologySuite.Geometries.OgcGeometryType.Point).WithMessage(localizer["Invalid Geometry Validation Message"]).WithErrorCode("INVALID_GEOM");
                RuleFor(f => f[0].Geometry.Centroid.X).GreaterThanOrEqualTo(0).LessThanOrEqualTo(700_000).WithMessage(localizer["Geometry Outside UK Validation Message"]).WithErrorCode("GEOM_OUTSIDE_BOUNDS");
                RuleFor(f => f[0].Geometry.Centroid.Y).GreaterThanOrEqualTo(0).LessThanOrEqualTo(1_300_000).WithMessage(localizer["Geometry Outside UK Validation Message"]).WithErrorCode("GEOM_OUTSIDE_BOUNDS");
            });

            RuleSet("Multi Point", () =>
            {
                RuleFor(g => g.Count).GreaterThanOrEqualTo(1).WithMessage(localizer["Invalid Geometry Validation Message"]).WithErrorCode("INVALID_GEOM");

                RuleFor(g => g[0].BoundingBox!.MinX).GreaterThanOrEqualTo(0).WithMessage(localizer["Geometry Outside UK Validation Message"]).WithErrorCode("GEOM_OUTSIDE_BOUNDS");
                RuleFor(g => g[0].BoundingBox!.MinY).GreaterThanOrEqualTo(0).WithMessage(localizer["Geometry Outside UK Validation Message"]).WithErrorCode("GEOM_OUTSIDE_BOUNDS");
                RuleFor(g => g[0].BoundingBox!.MaxX).LessThanOrEqualTo(700_000).WithMessage(localizer["Geometry Outside UK Validation Message"]).WithErrorCode("GEOM_OUTSIDE_BOUNDS");
                RuleFor(g => g[0].BoundingBox!.MaxY).LessThanOrEqualTo(1_300_000).WithMessage(localizer["Geometry Outside UK Validation Message"]).WithErrorCode("GEOM_OUTSIDE_BOUNDS");
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

                RuleFor(g => g).Must(g => g.All(f => f.Geometry.Envelope.Coordinates[0].X >= 0))
                    .WithMessage(localizer["Geometry Outside UK Validation Message"])
                    .WithErrorCode("GEOM_OUTSIDE_BOUNDS");

                RuleFor(g => g).Must(g => g.All(f => f.Geometry.Envelope.Coordinates[0].Y >= 0))
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

                RuleFor(g => g).Must(g => g.All(f => f.Geometry.Envelope.Coordinates[0].X >= 0))
                     .WithMessage(localizer["Geometry Outside UK Validation Message"])
                     .WithErrorCode("GEOM_OUTSIDE_BOUNDS");

                RuleFor(g => g).Must(g => g.All(f => f.Geometry.Envelope.Coordinates[0].Y >= 0))
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
            var point = features[0].Geometry.Centroid;
            point.SRID = 27700;
            
            var NearestRoute = await _geometryValidationService.GetNearestRoute(point);

            return (NearestRoute is not null);
        }

    }
}
