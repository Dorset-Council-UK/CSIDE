using CSIDE.Data.Models.Surveys;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class FixMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<SurveyStatus>(
                name: "Status",
                schema: "cside",
                table: "BridgeSurveys",
                type: "survey_status",
                nullable: false,
                defaultValueSql: "'incomplete'::survey_status",
                oldClrType: typeof(SurveyStatus),
                oldType: "survey_status",
                oldDefaultValue: SurveyStatus.Incomplete);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<SurveyStatus>(
                name: "Status",
                schema: "cside",
                table: "BridgeSurveys",
                type: "survey_status",
                nullable: false,
                defaultValue: SurveyStatus.Incomplete,
                oldClrType: typeof(SurveyStatus),
                oldType: "survey_status",
                oldDefaultValueSql: "'incomplete'::survey_status");
        }
    }
}
