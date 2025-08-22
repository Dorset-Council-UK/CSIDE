using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNameFromInfrastructureItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Infrastructure_InfrastructureTypes_InfrastructureTypeId",
                schema: "cside",
                table: "Infrastructure");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "cside",
                table: "Infrastructure");

            migrationBuilder.AlterColumn<int>(
                name: "InfrastructureTypeId",
                schema: "cside",
                table: "Infrastructure",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Infrastructure_InfrastructureTypes_InfrastructureTypeId",
                schema: "cside",
                table: "Infrastructure",
                column: "InfrastructureTypeId",
                principalSchema: "cside",
                principalTable: "InfrastructureTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Infrastructure_InfrastructureTypes_InfrastructureTypeId",
                schema: "cside",
                table: "Infrastructure");

            migrationBuilder.AlterColumn<int>(
                name: "InfrastructureTypeId",
                schema: "cside",
                table: "Infrastructure",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "cside",
                table: "Infrastructure",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Infrastructure_InfrastructureTypes_InfrastructureTypeId",
                schema: "cside",
                table: "Infrastructure",
                column: "InfrastructureTypeId",
                principalSchema: "cside",
                principalTable: "InfrastructureTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
