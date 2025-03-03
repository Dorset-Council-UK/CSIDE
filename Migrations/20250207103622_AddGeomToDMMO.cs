using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddGeomToDMMO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<MultiLineString>(
                name: "Geom",
                schema: "cside",
                table: "DMMOApplication",
                type: "geometry (multilinestring)",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Geom",
                schema: "cside",
                table: "DMMOApplication");
        }
    }
}
