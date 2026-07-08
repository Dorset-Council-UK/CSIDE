using CSIDE.Data.Models.Shared;
using NetTopologySuite.Geometries;

namespace CSIDE.Data.Helpers
{
    public static class PlaceGeometryHelper
    {
        public static Polygon CreateBBOXPolygonFromPlaceGeometry(GazetteerEntry place)
        {
            if (place.LocalType == "Postcode" || (place.MbrXMin == 0 && place.MbrXMax == 0 && place.MbrYMin == 0 && place.MbrYMax == 0))
            {
                //postcodes don't come with MBRs, so approximate by putting a 500m radius round the point. Not perfect but acceptable
                //For safety, also do this if the MBRs are returned as 0 (in case local type name changes for example)
                place.MbrXMin = place.GeometryX - 500;
                place.MbrXMax = place.GeometryX + 500;
                place.MbrYMin = place.GeometryY - 500;
                place.MbrYMax = place.GeometryY + 500;
            }
            var bboxPolygon = new Polygon(
                new LinearRing(
                    [
                        new(decimal.ToDouble(place.MbrXMin), decimal.ToDouble(place.MbrYMin)),
                                new(decimal.ToDouble(place.MbrXMin), decimal.ToDouble(place.MbrYMax)),
                                new(decimal.ToDouble(place.MbrXMax), decimal.ToDouble(place.MbrYMax)),
                                new(decimal.ToDouble(place.MbrXMax), decimal.ToDouble(place.MbrYMin)),
                                new(decimal.ToDouble(place.MbrXMin), decimal.ToDouble(place.MbrYMin)),
                    ]
                )
            )
            {
                SRID = 27700,
            };
            return bboxPolygon;
        }
    }
}
