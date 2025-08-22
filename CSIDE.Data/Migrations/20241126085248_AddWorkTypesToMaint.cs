using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkTypesToMaint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkTypes",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobWorkTypes",
                schema: "cside",
                columns: table => new
                {
                    JobId = table.Column<int>(type: "integer", nullable: false),
                    WorkTypeId = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobWorkTypes", x => new { x.JobId, x.WorkTypeId });
                    table.ForeignKey(
                        name: "FK_JobWorkTypes_MaintenanceJobs_JobId",
                        column: x => x.JobId,
                        principalSchema: "cside",
                        principalTable: "MaintenanceJobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobWorkTypes_WorkTypes_WorkTypeId",
                        column: x => x.WorkTypeId,
                        principalSchema: "cside",
                        principalTable: "WorkTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobWorkTypes_WorkTypeId",
                schema: "cside",
                table: "JobWorkTypes",
                column: "WorkTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobWorkTypes",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "WorkTypes",
                schema: "cside");
        }
    }
}
