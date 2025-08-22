using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class ChangeLandownerDepositGeomToMultiPolygon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<MultiPolygon>(
                name: "Geom",
                schema: "cside",
                table: "LandownerDeposits",
                type: "geometry (multipolygon)",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geometry (point)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Point>(
                name: "Geom",
                schema: "cside",
                table: "LandownerDeposits",
                type: "geometry (point)",
                nullable: false,
                oldClrType: typeof(MultiPolygon),
                oldType: "geometry (multipolygon)");
        }
    }
}
