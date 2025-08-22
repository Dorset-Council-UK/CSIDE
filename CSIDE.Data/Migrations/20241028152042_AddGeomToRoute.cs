using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddGeomToRoute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceJobs_Routes_RouteId",
                schema: "cside",
                table: "MaintenanceJobs");

            migrationBuilder.AddColumn<LineString>(
                name: "Geom",
                schema: "cside",
                table: "Routes",
                type: "geometry (linestring)",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "RouteId",
                schema: "cside",
                table: "MaintenanceJobs",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceJobs_Routes_RouteId",
                schema: "cside",
                table: "MaintenanceJobs",
                column: "RouteId",
                principalSchema: "cside",
                principalTable: "Routes",
                principalColumn: "RouteCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceJobs_Routes_RouteId",
                schema: "cside",
                table: "MaintenanceJobs");

            migrationBuilder.DropColumn(
                name: "Geom",
                schema: "cside",
                table: "Routes");

            migrationBuilder.AlterColumn<string>(
                name: "RouteId",
                schema: "cside",
                table: "MaintenanceJobs",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceJobs_Routes_RouteId",
                schema: "cside",
                table: "MaintenanceJobs",
                column: "RouteId",
                principalSchema: "cside",
                principalTable: "Routes",
                principalColumn: "RouteCode",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
