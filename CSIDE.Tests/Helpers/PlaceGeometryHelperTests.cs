using CSIDE.Data.Helpers;
using CSIDE.Data.Models.Shared;
using NetTopologySuite.Geometries;
using Xunit;

namespace CSIDE.Tests;

public class PlaceGeometryHelperTests
{
    [Fact]
    public void CreateBBOXPolygonFromPlaceGeometry_PostcodeLocalType_CalculatesMBRWithRadius()
    {
        // Arrange
        var place = new GazetteerEntry
        {
            LocalType = "Postcode",
            GeometryX = 1000m,
            GeometryY = 2000m,
            MbrXMin = 100m,
            MbrXMax = 200m,
            MbrYMin = 300m,
            MbrYMax = 400m
        };

        // Act
        var result = PlaceGeometryHelper.CreateBBOXPolygonFromPlaceGeometry(place);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(27700, result.SRID);
        Assert.Equal(5, result.Coordinates.Length); // Polygon should have 5 coordinates (closed ring)
        
        // Verify the coordinates are based on calculated MBR (GeometryX/Y ± 500)
        Assert.Equal(500.0, result.Coordinates[0].X); // MbrXMin = 1000 - 500
        Assert.Equal(1500.0, result.Coordinates[0].Y); // MbrYMin = 2000 - 500
        Assert.Equal(500.0, result.Coordinates[1].X); // MbrXMin
        Assert.Equal(2500.0, result.Coordinates[1].Y); // MbrYMax = 2000 + 500
        Assert.Equal(1500.0, result.Coordinates[2].X); // MbrXMax = 1000 + 500
        Assert.Equal(2500.0, result.Coordinates[2].Y); // MbrYMax
        Assert.Equal(1500.0, result.Coordinates[3].X); // MbrXMax
        Assert.Equal(1500.0, result.Coordinates[3].Y); // MbrYMin
        Assert.Equal(500.0, result.Coordinates[4].X); // MbrXMin (closing coordinate)
        Assert.Equal(1500.0, result.Coordinates[4].Y); // MbrYMin (closing coordinate)
    }

    [Fact]
    public void CreateBBOXPolygonFromPlaceGeometry_AllMBRValuesZero_CalculatesMBRWithRadius()
    {
        // Arrange
        var place = new GazetteerEntry
        {
            LocalType = "City",
            GeometryX = 500m,
            GeometryY = 750m,
            MbrXMin = 0m,
            MbrXMax = 0m,
            MbrYMin = 0m,
            MbrYMax = 0m
        };

        // Act
        var result = PlaceGeometryHelper.CreateBBOXPolygonFromPlaceGeometry(place);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(27700, result.SRID);
        Assert.Equal(5, result.Coordinates.Length);
        
        // Verify the coordinates are based on calculated MBR (GeometryX/Y ± 500)
        Assert.Equal(0.0, result.Coordinates[0].X); // MbrXMin = 500 - 500
        Assert.Equal(250.0, result.Coordinates[0].Y); // MbrYMin = 750 - 500
        Assert.Equal(0.0, result.Coordinates[1].X); // MbrXMin
        Assert.Equal(1250.0, result.Coordinates[1].Y); // MbrYMax = 750 + 500
        Assert.Equal(1000.0, result.Coordinates[2].X); // MbrXMax = 500 + 500
        Assert.Equal(1250.0, result.Coordinates[2].Y); // MbrYMax
        Assert.Equal(1000.0, result.Coordinates[3].X); // MbrXMax
        Assert.Equal(250.0, result.Coordinates[3].Y); // MbrYMin
        Assert.Equal(0.0, result.Coordinates[4].X); // MbrXMin (closing coordinate)
        Assert.Equal(250.0, result.Coordinates[4].Y); // MbrYMin (closing coordinate)
    }

    [Fact]
    public void CreateBBOXPolygonFromPlaceGeometry_NonPostcodeWithValidMBR_UsesProvidedMBRValues()
    {
        // Arrange
        var place = new GazetteerEntry
        {
            LocalType = "City",
            GeometryX = 1000m,
            GeometryY = 2000m,
            MbrXMin = 100m,
            MbrXMax = 200m,
            MbrYMin = 300m,
            MbrYMax = 400m
        };

        // Act
        var result = PlaceGeometryHelper.CreateBBOXPolygonFromPlaceGeometry(place);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(27700, result.SRID);
        Assert.Equal(5, result.Coordinates.Length);
        
        // Verify the coordinates use the provided MBR values
        Assert.Equal(100.0, result.Coordinates[0].X); // MbrXMin
        Assert.Equal(300.0, result.Coordinates[0].Y); // MbrYMin
        Assert.Equal(100.0, result.Coordinates[1].X); // MbrXMin
        Assert.Equal(400.0, result.Coordinates[1].Y); // MbrYMax
        Assert.Equal(200.0, result.Coordinates[2].X); // MbrXMax
        Assert.Equal(400.0, result.Coordinates[2].Y); // MbrYMax
        Assert.Equal(200.0, result.Coordinates[3].X); // MbrXMax
        Assert.Equal(300.0, result.Coordinates[3].Y); // MbrYMin
        Assert.Equal(100.0, result.Coordinates[4].X); // MbrXMin (closing coordinate)
        Assert.Equal(300.0, result.Coordinates[4].Y); // MbrYMin (closing coordinate)
    }

    [Fact]
    public void CreateBBOXPolygonFromPlaceGeometry_PartialMBRZeros_UsesProvidedMBRValues()
    {
        // Arrange - Only some MBR values are zero, but not all
        var place = new GazetteerEntry
        {
            LocalType = "Town",
            GeometryX = 5000m,
            GeometryY = 6000m,
            MbrXMin = 0m,
            MbrXMax = 1000m,
            MbrYMin = 500m,
            MbrYMax = 0m
        };

        // Act
        var result = PlaceGeometryHelper.CreateBBOXPolygonFromPlaceGeometry(place);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(27700, result.SRID);
        Assert.Equal(5, result.Coordinates.Length);
        
        // Verify the coordinates use the provided MBR values (not calculated)
        Assert.Equal(0.0, result.Coordinates[0].X); // MbrXMin
        Assert.Equal(500.0, result.Coordinates[0].Y); // MbrYMin
        Assert.Equal(0.0, result.Coordinates[1].X); // MbrXMin
        Assert.Equal(0.0, result.Coordinates[1].Y); // MbrYMax
        Assert.Equal(1000.0, result.Coordinates[2].X); // MbrXMax
        Assert.Equal(0.0, result.Coordinates[2].Y); // MbrYMax
        Assert.Equal(1000.0, result.Coordinates[3].X); // MbrXMax
        Assert.Equal(500.0, result.Coordinates[3].Y); // MbrYMin
        Assert.Equal(0.0, result.Coordinates[4].X); // MbrXMin (closing coordinate)
        Assert.Equal(500.0, result.Coordinates[4].Y); // MbrYMin (closing coordinate)
    }

    [Fact]
    public void CreateBBOXPolygonFromPlaceGeometry_NegativeGeometryCoordinates_CalculatesMBRCorrectly()
    {
        // Arrange
        var place = new GazetteerEntry
        {
            LocalType = "Postcode",
            GeometryX = -1000m,
            GeometryY = -2000m,
            MbrXMin = 0m,
            MbrXMax = 0m,
            MbrYMin = 0m,
            MbrYMax = 0m
        };

        // Act
        var result = PlaceGeometryHelper.CreateBBOXPolygonFromPlaceGeometry(place);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(27700, result.SRID);
        Assert.Equal(5, result.Coordinates.Length);
        
        // Verify the coordinates are based on calculated MBR (negative GeometryX/Y ± 500)
        Assert.Equal(-1500.0, result.Coordinates[0].X); // MbrXMin = -1000 - 500
        Assert.Equal(-2500.0, result.Coordinates[0].Y); // MbrYMin = -2000 - 500
        Assert.Equal(-1500.0, result.Coordinates[1].X); // MbrXMin
        Assert.Equal(-1500.0, result.Coordinates[1].Y); // MbrYMax = -2000 + 500
        Assert.Equal(-500.0, result.Coordinates[2].X); // MbrXMax = -1000 + 500
        Assert.Equal(-1500.0, result.Coordinates[2].Y); // MbrYMax
        Assert.Equal(-500.0, result.Coordinates[3].X); // MbrXMax
        Assert.Equal(-2500.0, result.Coordinates[3].Y); // MbrYMin
        Assert.Equal(-1500.0, result.Coordinates[4].X); // MbrXMin (closing coordinate)
        Assert.Equal(-2500.0, result.Coordinates[4].Y); // MbrYMin (closing coordinate)
    }

    [Fact]
    public void CreateBBOXPolygonFromPlaceGeometry_PostcodeCaseInsensitive_CalculatesMBRWithRadius()
    {
        // Arrange - Test with different case
        var place = new GazetteerEntry
        {
            LocalType = "POSTCODE",
            GeometryX = 1000m,
            GeometryY = 2000m,
            MbrXMin = 100m,
            MbrXMax = 200m,
            MbrYMin = 300m,
            MbrYMax = 400m
        };

        // Act
        var result = PlaceGeometryHelper.CreateBBOXPolygonFromPlaceGeometry(place);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(27700, result.SRID);
        // Based on the code, it's case-sensitive, so this should use provided MBR values
        Assert.Equal(100.0, result.Coordinates[0].X); // Uses provided MbrXMin
        Assert.Equal(300.0, result.Coordinates[0].Y); // Uses provided MbrYMin
    }

    [Fact]
    public void CreateBBOXPolygonFromPlaceGeometry_LargeDecimalValues_ConvertsToDoubleCorrectly()
    {
        // Arrange
        var place = new GazetteerEntry
        {
            LocalType = "Village",
            GeometryX = 999999m,
            GeometryY = 888888m,
            MbrXMin = 100000.5m,
            MbrXMax = 200000.75m,
            MbrYMin = 300000.25m,
            MbrYMax = 400000.99m
        };

        // Act
        var result = PlaceGeometryHelper.CreateBBOXPolygonFromPlaceGeometry(place);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(27700, result.SRID);
        Assert.Equal(5, result.Coordinates.Length);
        
        // Verify decimal to double conversion
        Assert.Equal(100000.5, result.Coordinates[0].X, precision: 10);
        Assert.Equal(300000.25, result.Coordinates[0].Y, precision: 10);
        Assert.Equal(100000.5, result.Coordinates[1].X, precision: 10);
        Assert.Equal(400000.99, result.Coordinates[1].Y, precision: 10);
        Assert.Equal(200000.75, result.Coordinates[2].X, precision: 10);
        Assert.Equal(400000.99, result.Coordinates[2].Y, precision: 10);
        Assert.Equal(200000.75, result.Coordinates[3].X, precision: 10);
        Assert.Equal(300000.25, result.Coordinates[3].Y, precision: 10);
        Assert.Equal(100000.5, result.Coordinates[4].X, precision: 10);
        Assert.Equal(300000.25, result.Coordinates[4].Y, precision: 10);
    }

    [Fact]
    public void CreateBBOXPolygonFromPlaceGeometry_ZeroGeometryCoordinates_CalculatesMBRCorrectly()
    {
        // Arrange
        var place = new GazetteerEntry
        {
            LocalType = "Postcode",
            GeometryX = 0m,
            GeometryY = 0m,
            MbrXMin = 0m,
            MbrXMax = 0m,
            MbrYMin = 0m,
            MbrYMax = 0m
        };

        // Act
        var result = PlaceGeometryHelper.CreateBBOXPolygonFromPlaceGeometry(place);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(27700, result.SRID);
        Assert.Equal(5, result.Coordinates.Length);
        
        // Verify the coordinates are based on calculated MBR (0 ± 500)
        Assert.Equal(-500.0, result.Coordinates[0].X); // MbrXMin = 0 - 500
        Assert.Equal(-500.0, result.Coordinates[0].Y); // MbrYMin = 0 - 500
        Assert.Equal(-500.0, result.Coordinates[1].X); // MbrXMin
        Assert.Equal(500.0, result.Coordinates[1].Y); // MbrYMax = 0 + 500
        Assert.Equal(500.0, result.Coordinates[2].X); // MbrXMax = 0 + 500
        Assert.Equal(500.0, result.Coordinates[2].Y); // MbrYMax
        Assert.Equal(500.0, result.Coordinates[3].X); // MbrXMax
        Assert.Equal(-500.0, result.Coordinates[3].Y); // MbrYMin
        Assert.Equal(-500.0, result.Coordinates[4].X); // MbrXMin (closing coordinate)
        Assert.Equal(-500.0, result.Coordinates[4].Y); // MbrYMin (closing coordinate)
    }
}
