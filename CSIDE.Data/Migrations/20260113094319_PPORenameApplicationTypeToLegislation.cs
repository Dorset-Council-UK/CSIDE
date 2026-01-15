using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class PPORenameApplicationTypeToLegislation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PPOApplication_PPOApplicationTypes_ApplicationTypeId",
                schema: "cside",
                table: "PPOApplication");

            migrationBuilder.DropTable(
                name: "PPOApplicationTypes",
                schema: "cside");

            migrationBuilder.RenameColumn(
                name: "ApplicationTypeId",
                schema: "cside",
                table: "PPOApplication",
                newName: "LegislationId");

            migrationBuilder.RenameIndex(
                name: "IX_PPOApplication_ApplicationTypeId",
                schema: "cside",
                table: "PPOApplication",
                newName: "IX_PPOApplication_LegislationId");

            migrationBuilder.CreateTable(
                name: "PPOApplicationLegislation",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPOApplicationLegislation", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceJobSubscribers_MaintenanceJobs_JobId",
                schema: "cside",
                table: "MaintenanceJobSubscribers",
                column: "JobId",
                principalSchema: "cside",
                principalTable: "MaintenanceJobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PPOApplication_PPOApplicationLegislation_LegislationId",
                schema: "cside",
                table: "PPOApplication",
                column: "LegislationId",
                principalSchema: "cside",
                principalTable: "PPOApplicationLegislation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceJobSubscribers_MaintenanceJobs_JobId",
                schema: "cside",
                table: "MaintenanceJobSubscribers");

            migrationBuilder.DropForeignKey(
                name: "FK_PPOApplication_PPOApplicationLegislation_LegislationId",
                schema: "cside",
                table: "PPOApplication");

            migrationBuilder.DropTable(
                name: "PPOApplicationLegislation",
                schema: "cside");

            migrationBuilder.RenameColumn(
                name: "LegislationId",
                schema: "cside",
                table: "PPOApplication",
                newName: "ApplicationTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_PPOApplication_LegislationId",
                schema: "cside",
                table: "PPOApplication",
                newName: "IX_PPOApplication_ApplicationTypeId");

            migrationBuilder.CreateTable(
                name: "PPOApplicationTypes",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPOApplicationTypes", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_PPOApplication_PPOApplicationTypes_ApplicationTypeId",
                schema: "cside",
                table: "PPOApplication",
                column: "ApplicationTypeId",
                principalSchema: "cside",
                principalTable: "PPOApplicationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
