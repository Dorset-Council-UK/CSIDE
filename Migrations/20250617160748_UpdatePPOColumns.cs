using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePPOColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DirectionOfSecStateId",
                schema: "cside",
                table: "PPOApplication",
                newName: "Charge");

            migrationBuilder.RenameColumn(
                name: "DateOfDirectionOfSecState",
                schema: "cside",
                table: "PPOApplication",
                newName: "ReceivedDate");

            migrationBuilder.RenameColumn(
                name: "ClaimedStatusId",
                schema: "cside",
                table: "PPOApplication",
                newName: "PriorityId");

            migrationBuilder.RenameColumn(
                name: "ApplicationDate",
                schema: "cside",
                table: "PPOApplication",
                newName: "InspectionCertificationDate");

            migrationBuilder.AddColumn<string>(
                name: "BoxNumber",
                schema: "cside",
                table: "PPOApplication",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CouncilLandAffected",
                schema: "cside",
                table: "PPOApplication",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<LocalDate>(
                name: "DateOfDirection",
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

            migrationBuilder.CreateTable(
                name: "PPOApplicationPriorities",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPOApplicationPriorities", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PPOApplication_PriorityId",
                schema: "cside",
                table: "PPOApplication",
                column: "PriorityId");

            migrationBuilder.AddForeignKey(
                name: "FK_PPOApplication_PPOApplicationPriorities_PriorityId",
                schema: "cside",
                table: "PPOApplication",
                column: "PriorityId",
                principalSchema: "cside",
                principalTable: "PPOApplicationPriorities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PPOApplication_PPOApplicationPriorities_PriorityId",
                schema: "cside",
                table: "PPOApplication");

            migrationBuilder.DropTable(
                name: "PPOApplicationPriorities",
                schema: "cside");

            migrationBuilder.DropIndex(
                name: "IX_PPOApplication_PriorityId",
                schema: "cside",
                table: "PPOApplication");

            migrationBuilder.DropColumn(
                name: "BoxNumber",
                schema: "cside",
                table: "PPOApplication");

            migrationBuilder.DropColumn(
                name: "CouncilLandAffected",
                schema: "cside",
                table: "PPOApplication");

            migrationBuilder.DropColumn(
                name: "DateOfDirection",
                schema: "cside",
                table: "PPOApplication");

            migrationBuilder.DropColumn(
                name: "InspectionCertification",
                schema: "cside",
                table: "PPOApplication");

            migrationBuilder.RenameColumn(
                name: "ReceivedDate",
                schema: "cside",
                table: "PPOApplication",
                newName: "DateOfDirectionOfSecState");

            migrationBuilder.RenameColumn(
                name: "PriorityId",
                schema: "cside",
                table: "PPOApplication",
                newName: "ClaimedStatusId");

            migrationBuilder.RenameColumn(
                name: "InspectionCertificationDate",
                schema: "cside",
                table: "PPOApplication",
                newName: "ApplicationDate");

            migrationBuilder.RenameColumn(
                name: "Charge",
                schema: "cside",
                table: "PPOApplication",
                newName: "DirectionOfSecStateId");
        }
    }
}
