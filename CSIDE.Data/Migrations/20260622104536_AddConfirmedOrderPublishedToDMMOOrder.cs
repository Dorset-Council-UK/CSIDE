using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace CSIDE.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddConfirmedOrderPublishedToDMMOOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<LocalDate>(
                name: "confirmation_published_date",
                schema: "cside",
                table: "dmmo_orders",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "confirmation_published_date",
                schema: "cside",
                table: "dmmo_orders");
        }
    }
}
