using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class PPOMovePropertiesFromApplicationsToOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmationPublishedDate",
                schema: "cside",
                table: "PPOApplication");

            migrationBuilder.DropColumn(
                name: "InspectionCertification",
                schema: "cside",
                table: "PPOApplication");

            migrationBuilder.DropColumn(
                name: "InspectionCertificationDate",
                schema: "cside",
                table: "PPOApplication");

            migrationBuilder.AddColumn<LocalDate>(
                name: "ConfirmationPublishedDate",
                schema: "cside",
                table: "PPOOrders",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "InspectionCertification",
                schema: "cside",
                table: "PPOOrders",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<LocalDate>(
                name: "InspectionCertificationDate",
                schema: "cside",
                table: "PPOOrders",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmationPublishedDate",
                schema: "cside",
                table: "PPOOrders");

            migrationBuilder.DropColumn(
                name: "InspectionCertification",
                schema: "cside",
                table: "PPOOrders");

            migrationBuilder.DropColumn(
                name: "InspectionCertificationDate",
                schema: "cside",
                table: "PPOOrders");

            migrationBuilder.AddColumn<LocalDate>(
                name: "ConfirmationPublishedDate",
                schema: "cside",
                table: "PPOApplication",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "InspectionCertification",
                schema: "cside",
                table: "PPOApplication",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<LocalDate>(
                name: "InspectionCertificationDate",
                schema: "cside",
                table: "PPOApplication",
                type: "date",
                nullable: true);
        }
    }
}
