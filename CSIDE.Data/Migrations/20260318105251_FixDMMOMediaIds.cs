using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSIDE.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixDMMOMediaIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_dmmo_media",
                schema: "cside",
                table: "dmmo_media");

            migrationBuilder.DropIndex(
                name: "ix_dmmo_media_dmmo_application_id",
                schema: "cside",
                table: "dmmo_media");

            migrationBuilder.DropColumn(
                name: "dmmo_id",
                schema: "cside",
                table: "dmmo_media");

            migrationBuilder.AddPrimaryKey(
                name: "pk_dmmo_media",
                schema: "cside",
                table: "dmmo_media",
                columns: new[] { "dmmo_application_id", "media_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_dmmo_media",
                schema: "cside",
                table: "dmmo_media");

            migrationBuilder.AddColumn<int>(
                name: "dmmo_id",
                schema: "cside",
                table: "dmmo_media",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "pk_dmmo_media",
                schema: "cside",
                table: "dmmo_media",
                columns: new[] { "dmmo_id", "media_id" });

            migrationBuilder.CreateIndex(
                name: "ix_dmmo_media_dmmo_application_id",
                schema: "cside",
                table: "dmmo_media",
                column: "dmmo_application_id");
        }
    }
}
