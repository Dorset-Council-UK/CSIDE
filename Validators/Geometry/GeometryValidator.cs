using CSIDE.Data;
using CSIDE.Data.Models.Maintenance;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Localization;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace CSIDE.Validators.Geometry
{
    public class GeometryValidator : AbstractValidator<FeatureCollection>
    {
        readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        readonly IStringLocalizer<Properties.Resources> _localizer;
        public GeometryValidator(IDbContextFactory<ApplicationDbContext> contextFactory, IStringLocalizer<Properties.Resources> localizer)
        {
            _localizer = localizer;
            _contextFactory = contextFactory;

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

        }

        private async Task<bool> PointOnRoute(FeatureCollection features, CancellationToken token)
        {
            var point = features.First().Geometry.Centroid;
            point.SRID = 27700;
            using var context = _contextFactory.CreateDbContext();
            //TODO - Spatial check for nearest route
            var Route = await context.Routes.Where(r => r.Geom.Distance(point) < 20).OrderBy(r => r.Geom.Distance(point)).FirstOrDefaultAsync(cancellationToken: token);

            return (Route is not null);
        }

    }
}
