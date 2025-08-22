using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddParishIdToInfrastructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParishId",
                schema: "cside",
                table: "Infrastructure",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Infrastructure_ParishId",
                schema: "cside",
                table: "Infrastructure",
                column: "ParishId");

            migrationBuilder.AddForeignKey(
                name: "FK_Infrastructure_Parishes_ParishId",
                schema: "cside",
                table: "Infrastructure",
                column: "ParishId",
                principalSchema: "cside",
                principalTable: "Parishes",
                principalColumn: "admin_unit_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Infrastructure_Parishes_ParishId",
                schema: "cside",
                table: "Infrastructure");

            migrationBuilder.DropIndex(
                name: "IX_Infrastructure_ParishId",
                schema: "cside",
                table: "Infrastructure");

            migrationBuilder.DropColumn(
                name: "ParishId",
                schema: "cside",
                table: "Infrastructure");
        }
    }
}
