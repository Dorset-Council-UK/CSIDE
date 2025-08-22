using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGeomOfRouteTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<LineString>(
                name: "Geom",
                schema: "cside",
                table: "Routes",
                type: "geometry (multilinestring)",
                nullable: false,
                oldClrType: typeof(LineString),
                oldType: "geometry (linestring)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<LineString>(
                name: "Geom",
                schema: "cside",
                table: "Routes",
                type: "geometry (linestring)",
                nullable: false,
                oldClrType: typeof(LineString),
                oldType: "geometry (multilinestring)");
        }
    }
}
