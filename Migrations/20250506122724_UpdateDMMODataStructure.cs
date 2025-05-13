using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDMMODataStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrimaryContact",
                schema: "cside",
                table: "DMMOApplication");

            migrationBuilder.DropColumn(
                name: "PrimaryContactUserId",
                schema: "cside",
                table: "DMMOApplication");

            migrationBuilder.AddColumn<bool>(
                name: "Appeal",
                schema: "cside",
                table: "DMMOApplication",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<LocalDate>(
                name: "AppealDate",
                schema: "cside",
                table: "DMMOApplication",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<LocalDate>(
                name: "DateOfDirectionOfSecState",
                schema: "cside",
                table: "DMMOApplication",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<LocalDate>(
                name: "DeterminationDate",
                schema: "cside",
                table: "DMMOApplication",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DirectionOfSecStateId",
                schema: "cside",
                table: "DMMOApplication",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DMMOApplicationDirectionsOfSecState",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMMOApplicationDirectionsOfSecState", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderDecisionsOfSecState",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDecisionsOfSecState", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderDeterminationProcesses",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDeterminationProcesses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DMMOOrders",
                schema: "cside",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    ApplicationId = table.Column<int>(type: "integer", nullable: false),
                    ObjectionsEndDate = table.Column<LocalDate>(type: "date", nullable: true),
                    ObjectionsReceived = table.Column<bool>(type: "boolean", nullable: true),
                    ObjectionsWithdrawn = table.Column<bool>(type: "boolean", nullable: true),
                    DeterminationProcessId = table.Column<int>(type: "integer", nullable: true),
                    DecisionOfSecStateId = table.Column<int>(type: "integer", nullable: true),
                    DateConfirmed = table.Column<LocalDate>(type: "date", nullable: true),
                    DateSealed = table.Column<LocalDate>(type: "date", nullable: true),
                    DatePublished = table.Column<LocalDate>(type: "date", nullable: true),
                    SubmitToPINS = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMMOOrders", x => new { x.OrderId, x.ApplicationId });
                    table.ForeignKey(
                        name: "FK_DMMOOrders_DMMOApplication_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "cside",
                        principalTable: "DMMOApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DMMOOrders_OrderDecisionsOfSecState_DecisionOfSecStateId",
                        column: x => x.DecisionOfSecStateId,
                        principalSchema: "cside",
                        principalTable: "OrderDecisionsOfSecState",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DMMOOrders_OrderDeterminationProcesses_DeterminationProcess~",
                        column: x => x.DeterminationProcessId,
                        principalSchema: "cside",
                        principalTable: "OrderDeterminationProcesses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DMMOApplication_DirectionOfSecStateId",
                schema: "cside",
                table: "DMMOApplication",
                column: "DirectionOfSecStateId");

            migrationBuilder.CreateIndex(
                name: "IX_DMMOOrders_ApplicationId",
                schema: "cside",
                table: "DMMOOrders",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_DMMOOrders_DecisionOfSecStateId",
                schema: "cside",
                table: "DMMOOrders",
                column: "DecisionOfSecStateId");

            migrationBuilder.CreateIndex(
                name: "IX_DMMOOrders_DeterminationProcessId",
                schema: "cside",
                table: "DMMOOrders",
                column: "DeterminationProcessId");

            migrationBuilder.AddForeignKey(
                name: "FK_DMMOApplication_DMMOApplicationDirectionsOfSecState_Directi~",
                schema: "cside",
                table: "DMMOApplication",
                column: "DirectionOfSecStateId",
                principalSchema: "cside",
                principalTable: "DMMOApplicationDirectionsOfSecState",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DMMOApplication_DMMOApplicationDirectionsOfSecState_Directi~",
                schema: "cside",
                table: "DMMOApplication");

            migrationBuilder.DropTable(
                name: "DMMOApplicationDirectionsOfSecState",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "DMMOOrders",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "OrderDecisionsOfSecState",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "OrderDeterminationProcesses",
                schema: "cside");

            migrationBuilder.DropIndex(
                name: "IX_DMMOApplication_DirectionOfSecStateId",
                schema: "cside",
                table: "DMMOApplication");

            migrationBuilder.DropColumn(
                name: "Appeal",
                schema: "cside",
                table: "DMMOApplication");

            migrationBuilder.DropColumn(
                name: "AppealDate",
                schema: "cside",
                table: "DMMOApplication");

            migrationBuilder.DropColumn(
                name: "DateOfDirectionOfSecState",
                schema: "cside",
                table: "DMMOApplication");

            migrationBuilder.DropColumn(
                name: "DeterminationDate",
                schema: "cside",
                table: "DMMOApplication");

            migrationBuilder.DropColumn(
                name: "DirectionOfSecStateId",
                schema: "cside",
                table: "DMMOApplication");

            migrationBuilder.AddColumn<string>(
                name: "PrimaryContact",
                schema: "cside",
                table: "DMMOApplication",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryContactUserId",
                schema: "cside",
                table: "DMMOApplication",
                type: "text",
                nullable: true);
        }
    }
}
