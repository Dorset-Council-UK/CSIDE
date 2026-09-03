using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace CSIDE.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDeterminationDateFromDMMO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "determination_date",
                schema: "cside",
                table: "dmmo_application");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<LocalDate>(
                name: "determination_date",
                schema: "cside",
                table: "dmmo_application",
                type: "date",
                nullable: true);
        }
    }
}
