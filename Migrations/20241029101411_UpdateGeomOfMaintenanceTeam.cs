using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGeomOfMaintenanceTeam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Polygon>(
                name: "Geom",
                schema: "cside",
                table: "MaintenanceTeams",
                type: "geometry (polygon)",
                nullable: false,
                oldClrType: typeof(MultiPolygon),
                oldType: "geometry (multipolygon)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<MultiPolygon>(
                name: "Geom",
                schema: "cside",
                table: "MaintenanceTeams",
                type: "geometry (multipolygon)",
                nullable: false,
                oldClrType: typeof(Polygon),
                oldType: "geometry (polygon)");
        }
    }
}
