using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddGeomToMaintenanceTeam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<MultiPolygon>(
                name: "Geom",
                schema: "cside",
                table: "MaintenanceTeams",
                type: "geometry (multipolygon)",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Geom",
                schema: "cside",
                table: "MaintenanceTeams");
        }
    }
}
