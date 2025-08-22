using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class LandownerDepositGeomTypeChangedToPoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LandownerDepositType_LandownerDepositTypeNames_LandownerDep~",
                schema: "cside",
                table: "LandownerDepositType");

            migrationBuilder.DropForeignKey(
                name: "FK_LandownerDepositType_LandownerDeposits_LandownerDepositId",
                schema: "cside",
                table: "LandownerDepositType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LandownerDepositType",
                schema: "cside",
                table: "LandownerDepositType");

            migrationBuilder.RenameTable(
                name: "LandownerDepositType",
                schema: "cside",
                newName: "LandownerDepositTypes",
                newSchema: "cside");

            migrationBuilder.RenameIndex(
                name: "IX_LandownerDepositType_LandownerDepositTypeNameId",
                schema: "cside",
                table: "LandownerDepositTypes",
                newName: "IX_LandownerDepositTypes_LandownerDepositTypeNameId");

            migrationBuilder.AlterColumn<bool>(
                name: "MapCorrect",
                schema: "cside",
                table: "LandownerDeposits",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AlterColumn<Point>(
                name: "Geom",
                schema: "cside",
                table: "LandownerDeposits",
                type: "geometry (point)",
                nullable: false,
                oldClrType: typeof(MultiPolygon),
                oldType: "geometry (multipolygon)");

            migrationBuilder.AlterColumn<bool>(
                name: "FormCompleted",
                schema: "cside",
                table: "LandownerDeposits",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "FeePaid",
                schema: "cside",
                table: "LandownerDeposits",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "AllSigned",
                schema: "cside",
                table: "LandownerDeposits",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LandownerDepositTypes",
                schema: "cside",
                table: "LandownerDepositTypes",
                columns: ["LandownerDepositId", "LandownerDepositTypeNameId"]);

            migrationBuilder.AddForeignKey(
                name: "FK_LandownerDepositTypes_LandownerDepositTypeNames_LandownerDe~",
                schema: "cside",
                table: "LandownerDepositTypes",
                column: "LandownerDepositTypeNameId",
                principalSchema: "cside",
                principalTable: "LandownerDepositTypeNames",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LandownerDepositTypes_LandownerDeposits_LandownerDepositId",
                schema: "cside",
                table: "LandownerDepositTypes",
                column: "LandownerDepositId",
                principalSchema: "cside",
                principalTable: "LandownerDeposits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LandownerDepositTypes_LandownerDepositTypeNames_LandownerDe~",
                schema: "cside",
                table: "LandownerDepositTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_LandownerDepositTypes_LandownerDeposits_LandownerDepositId",
                schema: "cside",
                table: "LandownerDepositTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LandownerDepositTypes",
                schema: "cside",
                table: "LandownerDepositTypes");

            migrationBuilder.RenameTable(
                name: "LandownerDepositTypes",
                schema: "cside",
                newName: "LandownerDepositType",
                newSchema: "cside");

            migrationBuilder.RenameIndex(
                name: "IX_LandownerDepositTypes_LandownerDepositTypeNameId",
                schema: "cside",
                table: "LandownerDepositType",
                newName: "IX_LandownerDepositType_LandownerDepositTypeNameId");

            migrationBuilder.AlterColumn<bool>(
                name: "MapCorrect",
                schema: "cside",
                table: "LandownerDeposits",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<MultiPolygon>(
                name: "Geom",
                schema: "cside",
                table: "LandownerDeposits",
                type: "geometry (multipolygon)",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geometry (point)");

            migrationBuilder.AlterColumn<bool>(
                name: "FormCompleted",
                schema: "cside",
                table: "LandownerDeposits",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "FeePaid",
                schema: "cside",
                table: "LandownerDeposits",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "AllSigned",
                schema: "cside",
                table: "LandownerDeposits",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LandownerDepositType",
                schema: "cside",
                table: "LandownerDepositType",
                columns: ["LandownerDepositId", "LandownerDepositTypeNameId"]);

            migrationBuilder.AddForeignKey(
                name: "FK_LandownerDepositType_LandownerDepositTypeNames_LandownerDep~",
                schema: "cside",
                table: "LandownerDepositType",
                column: "LandownerDepositTypeNameId",
                principalSchema: "cside",
                principalTable: "LandownerDepositTypeNames",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LandownerDepositType_LandownerDeposits_LandownerDepositId",
                schema: "cside",
                table: "LandownerDepositType",
                column: "LandownerDepositId",
                principalSchema: "cside",
                principalTable: "LandownerDeposits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
