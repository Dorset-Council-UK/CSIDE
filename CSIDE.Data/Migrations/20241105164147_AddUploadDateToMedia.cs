using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddUploadDateToMedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Instant>(
                name: "UploadDate",
                schema: "cside",
                table: "Media",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UploadDate",
                schema: "cside",
                table: "Media");
        }
    }
}
