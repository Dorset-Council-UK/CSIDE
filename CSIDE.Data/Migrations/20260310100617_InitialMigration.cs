using System;
using System.Text.Json;
using CSIDE.Data.Models.Surveys;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CSIDE.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cside");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:cside.survey_status", "completed,incomplete,rejected,verified")
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateSequence(
                name: "SurveySequence",
                schema: "cside");

            migrationBuilder.CreateTable(
                name: "application_roles",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_application_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "audit_logs",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    entity_name = table.Column<string>(type: "text", nullable: false),
                    entity_id = table.Column<string>(type: "text", nullable: false),
                    secondary_entity_id = table.Column<string>(type: "text", nullable: true),
                    change_type = table.Column<string>(type: "text", nullable: false),
                    user_name = table.Column<string>(type: "text", nullable: false),
                    old_values = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    new_values = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    user_id = table.Column<string>(type: "text", nullable: true),
                    log_date = table.Column<Instant>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audit_logs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "conditions",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_conditions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "contact_types",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contact_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "dmmo_application_case_statuses",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dmmo_application_case_statuses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "dmmo_application_claimed_statuses",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dmmo_application_claimed_statuses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "dmmo_application_directions_of_sec_state",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dmmo_application_directions_of_sec_state", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "dmmo_application_types",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dmmo_application_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "dmmo_media_type",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    num_files_limit = table.Column<int>(type: "integer", nullable: false),
                    file_types_limit = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dmmo_media_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "infrastructure_types",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    is_bridge = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_infrastructure_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "landowner_deposit_media_types",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    num_files_limit = table.Column<int>(type: "integer", nullable: false),
                    file_types_limit = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_landowner_deposit_media_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "landowner_deposit_type_names",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_landowner_deposit_type_names", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "landowner_deposits",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    secondary_id = table.Column<int>(type: "integer", nullable: false),
                    received_date = table.Column<LocalDate>(type: "date", nullable: true),
                    location = table.Column<string>(type: "text", nullable: true),
                    website_notice_published = table.Column<LocalDate>(type: "date", nullable: true),
                    email_notice_sent = table.Column<LocalDate>(type: "date", nullable: true),
                    onsite_notice_erected = table.Column<LocalDate>(type: "date", nullable: true),
                    intended_effect = table.Column<string>(type: "text", nullable: true),
                    elapse_date = table.Column<LocalDate>(type: "date", nullable: true),
                    form_completed = table.Column<bool>(type: "boolean", nullable: false),
                    map_correct = table.Column<bool>(type: "boolean", nullable: false),
                    fee_paid = table.Column<bool>(type: "boolean", nullable: false),
                    all_signed = table.Column<bool>(type: "boolean", nullable: false),
                    date_acknowledged = table.Column<LocalDate>(type: "date", nullable: true),
                    cheque_receipt_number = table.Column<string>(type: "text", nullable: true),
                    cheque_paid_in_date = table.Column<LocalDate>(type: "date", nullable: true),
                    notice_drafted = table.Column<LocalDate>(type: "date", nullable: true),
                    sent_to_archive = table.Column<LocalDate>(type: "date", nullable: true),
                    archive_reference = table.Column<string>(type: "text", nullable: true),
                    website_entry_added = table.Column<LocalDate>(type: "date", nullable: true),
                    primary_contact = table.Column<string>(type: "text", nullable: true),
                    primary_contact_user_id = table.Column<string>(type: "text", nullable: true),
                    geom = table.Column<MultiPolygon>(type: "geometry(multipolygon, 27700)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_landowner_deposits", x => new { x.id, x.secondary_id });
                });

            migrationBuilder.CreateTable(
                name: "maintenance_job_priorities",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_maintenance_job_priorities", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "maintenance_job_statuses",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: false),
                    friendly_description = table.Column<string>(type: "text", nullable: false),
                    is_complete = table.Column<bool>(type: "boolean", nullable: false),
                    is_duplicate = table.Column<bool>(type: "boolean", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_maintenance_job_statuses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "maintenance_teams",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    geom = table.Column<Polygon>(type: "geometry(polygon, 27700)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_maintenance_teams", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "materials",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_wood = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_materials", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "media",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    upload_date = table.Column<Instant>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    url = table.Column<string>(type: "text", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_media", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "order_decisions_of_sec_state",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_order_decisions_of_sec_state", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "order_determination_processes",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_order_determination_processes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "parish_codes",
                schema: "cside",
                columns: table => new
                {
                    parish_id = table.Column<int>(type: "integer", nullable: false),
                    code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_parish_codes", x => new { x.parish_id, x.code });
                    table.ForeignKey(
                        name: "fk_parish_codes_parishes_parish_id",
                        column: x => x.parish_id,
                        principalSchema: "cside",
                        principalTable: "parishes",
                        principalColumn: "admin_unit_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ppo_application_case_statuses",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    is_closed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ppo_application_case_statuses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ppo_application_legislation",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ppo_application_legislation", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ppo_application_priorities",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ppo_application_priorities", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ppo_application_types",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ppo_application_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ppo_media_type",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    num_files_limit = table.Column<int>(type: "integer", nullable: false),
                    file_types_limit = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ppo_media_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "problem_types",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_problem_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "route_legal_statuses",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_route_legal_statuses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "route_operational_statuses",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    is_closed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_route_operational_statuses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "route_types",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_route_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "application_user_roles",
                schema: "cside",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "text", nullable: false),
                    application_role_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_application_user_roles", x => new { x.user_id, x.application_role_id });
                    table.ForeignKey(
                        name: "fk_application_user_roles_application_roles_application_role_id",
                        column: x => x.application_role_id,
                        principalSchema: "cside",
                        principalTable: "application_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contacts",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    email = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    primary_contact_no = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    secondary_contact_no = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    organisation_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    contact_type_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contacts", x => x.id);
                    table.ForeignKey(
                        name: "fk_contacts_contact_types_contact_type_id",
                        column: x => x.contact_type_id,
                        principalSchema: "cside",
                        principalTable: "contact_types",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "dmmo_application",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    application_date = table.Column<LocalDate>(type: "date", nullable: true),
                    received_date = table.Column<LocalDate>(type: "date", nullable: true),
                    is_public = table.Column<bool>(type: "boolean", nullable: false),
                    application_details = table.Column<string>(type: "text", nullable: false),
                    location_description = table.Column<string>(type: "text", nullable: true),
                    case_officer = table.Column<string>(type: "text", nullable: true),
                    case_officer_user_id = table.Column<string>(type: "text", nullable: true),
                    private_comments = table.Column<string>(type: "text", nullable: true),
                    public_comments = table.Column<string>(type: "text", nullable: true),
                    determination_date = table.Column<LocalDate>(type: "date", nullable: true),
                    appeal = table.Column<bool>(type: "boolean", nullable: true),
                    appeal_date = table.Column<LocalDate>(type: "date", nullable: true),
                    date_of_direction_of_sec_state = table.Column<LocalDate>(type: "date", nullable: true),
                    geom = table.Column<MultiLineString>(type: "geometry(multilinestring, 27700)", nullable: false),
                    case_status_id = table.Column<int>(type: "integer", nullable: false),
                    direction_of_sec_state_id = table.Column<int>(type: "integer", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dmmo_application", x => x.id);
                    table.ForeignKey(
                        name: "fk_dmmo_application_dmmo_application_case_statuses_case_status",
                        column: x => x.case_status_id,
                        principalSchema: "cside",
                        principalTable: "dmmo_application_case_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_dmmo_application_dmmo_application_directions_of_sec_state_d",
                        column: x => x.direction_of_sec_state_id,
                        principalSchema: "cside",
                        principalTable: "dmmo_application_directions_of_sec_state",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "landowner_deposit_addresses",
                schema: "cside",
                columns: table => new
                {
                    landowner_deposit_id = table.Column<int>(type: "integer", nullable: false),
                    landowner_deposit_secondary_id = table.Column<int>(type: "integer", nullable: false),
                    uprn = table.Column<long>(type: "bigint", nullable: false),
                    address = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_landowner_deposit_addresses", x => new { x.landowner_deposit_id, x.landowner_deposit_secondary_id, x.uprn });
                    table.ForeignKey(
                        name: "fk_landowner_deposit_addresses_landowner_deposits_landowner_de",
                        columns: x => new { x.landowner_deposit_id, x.landowner_deposit_secondary_id },
                        principalSchema: "cside",
                        principalTable: "landowner_deposits",
                        principalColumns: new[] { "id", "secondary_id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "landowner_deposit_events",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    landowner_deposit_id = table.Column<int>(type: "integer", nullable: false),
                    landowner_deposit_secondary_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<Instant>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    event_date = table.Column<LocalDate>(type: "date", nullable: false),
                    event_text = table.Column<string>(type: "text", nullable: false),
                    author_id = table.Column<string>(type: "text", nullable: true),
                    author_name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_landowner_deposit_events", x => x.id);
                    table.ForeignKey(
                        name: "fk_landowner_deposit_events_landowner_deposits_landowner_depos",
                        columns: x => new { x.landowner_deposit_id, x.landowner_deposit_secondary_id },
                        principalSchema: "cside",
                        principalTable: "landowner_deposits",
                        principalColumns: new[] { "id", "secondary_id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "landowner_deposit_parishes",
                schema: "cside",
                columns: table => new
                {
                    landowner_deposit_id = table.Column<int>(type: "integer", nullable: false),
                    landowner_deposit_secondary_id = table.Column<int>(type: "integer", nullable: false),
                    parish_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_landowner_deposit_parishes", x => new { x.landowner_deposit_id, x.landowner_deposit_secondary_id, x.parish_id });
                    table.ForeignKey(
                        name: "fk_landowner_deposit_parishes_landowner_deposits_landowner_dep",
                        columns: x => new { x.landowner_deposit_id, x.landowner_deposit_secondary_id },
                        principalSchema: "cside",
                        principalTable: "landowner_deposits",
                        principalColumns: new[] { "id", "secondary_id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_landowner_deposit_parishes_parishes_parish_id",
                        column: x => x.parish_id,
                        principalSchema: "cside",
                        principalTable: "parishes",
                        principalColumn: "admin_unit_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "landowner_deposit_types",
                schema: "cside",
                columns: table => new
                {
                    landowner_deposit_id = table.Column<int>(type: "integer", nullable: false),
                    landowner_deposit_secondary_id = table.Column<int>(type: "integer", nullable: false),
                    landowner_deposit_type_name_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_landowner_deposit_types", x => new { x.landowner_deposit_id, x.landowner_deposit_secondary_id, x.landowner_deposit_type_name_id });
                    table.ForeignKey(
                        name: "fk_landowner_deposit_types_landowner_deposit_type_names_landow",
                        column: x => x.landowner_deposit_type_name_id,
                        principalSchema: "cside",
                        principalTable: "landowner_deposit_type_names",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_landowner_deposit_types_landowner_deposits_landowner_deposi",
                        columns: x => new { x.landowner_deposit_id, x.landowner_deposit_secondary_id },
                        principalSchema: "cside",
                        principalTable: "landowner_deposits",
                        principalColumns: new[] { "id", "secondary_id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "maintenance_team_users",
                schema: "cside",
                columns: table => new
                {
                    team_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    is_lead = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_maintenance_team_users", x => new { x.team_id, x.user_id });
                    table.ForeignKey(
                        name: "fk_maintenance_team_users_maintenance_teams_team_id",
                        column: x => x.team_id,
                        principalSchema: "cside",
                        principalTable: "maintenance_teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "landowner_deposit_media",
                schema: "cside",
                columns: table => new
                {
                    landowner_deposit_id = table.Column<int>(type: "integer", nullable: false),
                    landowner_deposit_secondary_id = table.Column<int>(type: "integer", nullable: false),
                    media_id = table.Column<int>(type: "integer", nullable: false),
                    media_type_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_landowner_deposit_media", x => new { x.landowner_deposit_id, x.landowner_deposit_secondary_id, x.media_id });
                    table.ForeignKey(
                        name: "fk_landowner_deposit_media_landowner_deposit_media_types_media",
                        column: x => x.media_type_id,
                        principalSchema: "cside",
                        principalTable: "landowner_deposit_media_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_landowner_deposit_media_landowner_deposits_landowner_deposi",
                        columns: x => new { x.landowner_deposit_id, x.landowner_deposit_secondary_id },
                        principalSchema: "cside",
                        principalTable: "landowner_deposits",
                        principalColumns: new[] { "id", "secondary_id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_landowner_deposit_media_media_media_id",
                        column: x => x.media_id,
                        principalSchema: "cside",
                        principalTable: "media",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "survey_media",
                schema: "cside",
                columns: table => new
                {
                    survey_id = table.Column<int>(type: "integer", nullable: false),
                    media_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_survey_media", x => new { x.survey_id, x.media_id });
                    table.ForeignKey(
                        name: "fk_survey_media_media_media_id",
                        column: x => x.media_id,
                        principalSchema: "cside",
                        principalTable: "media",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ppo_application",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    received_date = table.Column<LocalDate>(type: "date", nullable: true),
                    is_public = table.Column<bool>(type: "boolean", nullable: false),
                    application_details = table.Column<string>(type: "text", nullable: false),
                    location_description = table.Column<string>(type: "text", nullable: true),
                    case_officer = table.Column<string>(type: "text", nullable: true),
                    case_officer_user_id = table.Column<string>(type: "text", nullable: true),
                    determination_date = table.Column<LocalDate>(type: "date", nullable: true),
                    council_land_affected = table.Column<bool>(type: "boolean", nullable: true),
                    charge = table.Column<decimal>(type: "numeric", nullable: true),
                    box_number = table.Column<string>(type: "text", nullable: true),
                    private_comments = table.Column<string>(type: "text", nullable: true),
                    public_comments = table.Column<string>(type: "text", nullable: true),
                    geom = table.Column<MultiLineString>(type: "geometry(multilinestring, 27700)", nullable: false),
                    legislation_id = table.Column<int>(type: "integer", nullable: false),
                    priority_id = table.Column<int>(type: "integer", nullable: false),
                    case_status_id = table.Column<int>(type: "integer", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ppo_application", x => x.id);
                    table.ForeignKey(
                        name: "fk_ppo_application_ppo_application_case_statuses_case_status_id",
                        column: x => x.case_status_id,
                        principalSchema: "cside",
                        principalTable: "ppo_application_case_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ppo_application_ppo_application_legislation_legislation_id",
                        column: x => x.legislation_id,
                        principalSchema: "cside",
                        principalTable: "ppo_application_legislation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ppo_application_ppo_application_priorities_priority_id",
                        column: x => x.priority_id,
                        principalSchema: "cside",
                        principalTable: "ppo_application_priorities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "routes",
                schema: "cside",
                columns: table => new
                {
                    route_code = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    closure_start_date = table.Column<LocalDate>(type: "date", nullable: true),
                    closure_end_date = table.Column<LocalDate>(type: "date", nullable: true),
                    closure_is_indefinite = table.Column<bool>(type: "boolean", nullable: false),
                    geom = table.Column<MultiLineString>(type: "geometry(multilinestring, 27700)", nullable: false),
                    route_type_id = table.Column<int>(type: "integer", nullable: false),
                    operational_status_id = table.Column<int>(type: "integer", nullable: false),
                    legal_status_id = table.Column<int>(type: "integer", nullable: false),
                    maintenance_team_id = table.Column<int>(type: "integer", nullable: true),
                    parish_id = table.Column<int>(type: "integer", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_routes", x => x.route_code);
                    table.ForeignKey(
                        name: "fk_routes_maintenance_teams_maintenance_team_id",
                        column: x => x.maintenance_team_id,
                        principalSchema: "cside",
                        principalTable: "maintenance_teams",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_routes_parishes_parish_id",
                        column: x => x.parish_id,
                        principalSchema: "cside",
                        principalTable: "parishes",
                        principalColumn: "admin_unit_id");
                    table.ForeignKey(
                        name: "fk_routes_route_legal_statuses_legal_status_id",
                        column: x => x.legal_status_id,
                        principalSchema: "cside",
                        principalTable: "route_legal_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_routes_route_operational_statuses_operational_status_id",
                        column: x => x.operational_status_id,
                        principalSchema: "cside",
                        principalTable: "route_operational_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_routes_route_types_route_type_id",
                        column: x => x.route_type_id,
                        principalSchema: "cside",
                        principalTable: "route_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "landowner_deposit_contacts",
                schema: "cside",
                columns: table => new
                {
                    landowner_deposit_id = table.Column<int>(type: "integer", nullable: false),
                    landowner_deposit_secondary_id = table.Column<int>(type: "integer", nullable: false),
                    contact_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_landowner_deposit_contacts", x => new { x.landowner_deposit_id, x.landowner_deposit_secondary_id, x.contact_id });
                    table.ForeignKey(
                        name: "fk_landowner_deposit_contacts_contacts_contact_id",
                        column: x => x.contact_id,
                        principalSchema: "cside",
                        principalTable: "contacts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_landowner_deposit_contacts_landowner_deposits_landowner_dep",
                        columns: x => new { x.landowner_deposit_id, x.landowner_deposit_secondary_id },
                        principalSchema: "cside",
                        principalTable: "landowner_deposits",
                        principalColumns: new[] { "id", "secondary_id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "dmmo_addresses",
                schema: "cside",
                columns: table => new
                {
                    dmmo_application_id = table.Column<int>(type: "integer", nullable: false),
                    uprn = table.Column<long>(type: "bigint", nullable: false),
                    address = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dmmo_addresses", x => new { x.dmmo_application_id, x.uprn });
                    table.ForeignKey(
                        name: "fk_dmmo_addresses_dmmo_application_dmmo_application_id",
                        column: x => x.dmmo_application_id,
                        principalSchema: "cside",
                        principalTable: "dmmo_application",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "dmmo_claimed_statuses",
                schema: "cside",
                columns: table => new
                {
                    dmmo_application_id = table.Column<int>(type: "integer", nullable: false),
                    claimed_status_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dmmo_claimed_statuses", x => new { x.dmmo_application_id, x.claimed_status_id });
                    table.ForeignKey(
                        name: "fk_dmmo_claimed_statuses_dmmo_application_claimed_statuses_cla",
                        column: x => x.claimed_status_id,
                        principalSchema: "cside",
                        principalTable: "dmmo_application_claimed_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_dmmo_claimed_statuses_dmmo_application_dmmo_application_id",
                        column: x => x.dmmo_application_id,
                        principalSchema: "cside",
                        principalTable: "dmmo_application",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "dmmo_contact",
                schema: "cside",
                columns: table => new
                {
                    dmmo_application_id = table.Column<int>(type: "integer", nullable: false),
                    contact_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dmmo_contact", x => new { x.dmmo_application_id, x.contact_id });
                    table.ForeignKey(
                        name: "fk_dmmo_contact_contacts_contact_id",
                        column: x => x.contact_id,
                        principalSchema: "cside",
                        principalTable: "contacts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_dmmo_contact_dmmo_application_dmmo_application_id",
                        column: x => x.dmmo_application_id,
                        principalSchema: "cside",
                        principalTable: "dmmo_application",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "dmmo_events",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    dmmo_application_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<Instant>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    event_date = table.Column<LocalDate>(type: "date", nullable: false),
                    event_text = table.Column<string>(type: "text", nullable: false),
                    author_id = table.Column<string>(type: "text", nullable: true),
                    author_name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dmmo_events", x => x.id);
                    table.ForeignKey(
                        name: "fk_dmmo_events_dmmo_application_dmmo_application_id",
                        column: x => x.dmmo_application_id,
                        principalSchema: "cside",
                        principalTable: "dmmo_application",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "dmmo_media",
                schema: "cside",
                columns: table => new
                {
                    dmmo_id = table.Column<int>(type: "integer", nullable: false),
                    media_id = table.Column<int>(type: "integer", nullable: false),
                    media_type_id = table.Column<int>(type: "integer", nullable: false),
                    dmmo_application_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dmmo_media", x => new { x.dmmo_id, x.media_id });
                    table.ForeignKey(
                        name: "fk_dmmo_media_dmmo_application_dmmo_application_id",
                        column: x => x.dmmo_application_id,
                        principalSchema: "cside",
                        principalTable: "dmmo_application",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_dmmo_media_dmmo_media_type_media_type_id",
                        column: x => x.media_type_id,
                        principalSchema: "cside",
                        principalTable: "dmmo_media_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_dmmo_media_media_media_id",
                        column: x => x.media_id,
                        principalSchema: "cside",
                        principalTable: "media",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "dmmo_orders",
                schema: "cside",
                columns: table => new
                {
                    order_id = table.Column<int>(type: "integer", nullable: false),
                    dmmo_application_id = table.Column<int>(type: "integer", nullable: false),
                    objections_end_date = table.Column<LocalDate>(type: "date", nullable: true),
                    objections_received = table.Column<bool>(type: "boolean", nullable: true),
                    objections_withdrawn = table.Column<bool>(type: "boolean", nullable: true),
                    determination_process_id = table.Column<int>(type: "integer", nullable: true),
                    decision_of_sec_state_id = table.Column<int>(type: "integer", nullable: true),
                    date_confirmed = table.Column<LocalDate>(type: "date", nullable: true),
                    date_sealed = table.Column<LocalDate>(type: "date", nullable: true),
                    date_published = table.Column<LocalDate>(type: "date", nullable: true),
                    submit_to_pins = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dmmo_orders", x => new { x.order_id, x.dmmo_application_id });
                    table.ForeignKey(
                        name: "fk_dmmo_orders_dmmo_application_dmmo_application_id",
                        column: x => x.dmmo_application_id,
                        principalSchema: "cside",
                        principalTable: "dmmo_application",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_dmmo_orders_order_decisions_of_sec_state_decision_of_sec_st",
                        column: x => x.decision_of_sec_state_id,
                        principalSchema: "cside",
                        principalTable: "order_decisions_of_sec_state",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_dmmo_orders_order_determination_processes_determination_pro",
                        column: x => x.determination_process_id,
                        principalSchema: "cside",
                        principalTable: "order_determination_processes",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "dmmo_parish",
                schema: "cside",
                columns: table => new
                {
                    dmmo_application_id = table.Column<int>(type: "integer", nullable: false),
                    parish_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dmmo_parish", x => new { x.dmmo_application_id, x.parish_id });
                    table.ForeignKey(
                        name: "fk_dmmo_parish_dmmo_application_dmmo_application_id",
                        column: x => x.dmmo_application_id,
                        principalSchema: "cside",
                        principalTable: "dmmo_application",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_dmmo_parish_parishes_parish_id",
                        column: x => x.parish_id,
                        principalSchema: "cside",
                        principalTable: "parishes",
                        principalColumn: "admin_unit_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "dmmo_types",
                schema: "cside",
                columns: table => new
                {
                    dmmo_application_id = table.Column<int>(type: "integer", nullable: false),
                    application_type_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dmmo_types", x => new { x.dmmo_application_id, x.application_type_id });
                    table.ForeignKey(
                        name: "fk_dmmo_types_dmmo_application_dmmo_application_id",
                        column: x => x.dmmo_application_id,
                        principalSchema: "cside",
                        principalTable: "dmmo_application",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_dmmo_types_dmmo_application_types_application_type_id",
                        column: x => x.application_type_id,
                        principalSchema: "cside",
                        principalTable: "dmmo_application_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ppo_contact",
                schema: "cside",
                columns: table => new
                {
                    ppo_application_id = table.Column<int>(type: "integer", nullable: false),
                    contact_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ppo_contact", x => new { x.ppo_application_id, x.contact_id });
                    table.ForeignKey(
                        name: "fk_ppo_contact_contacts_contact_id",
                        column: x => x.contact_id,
                        principalSchema: "cside",
                        principalTable: "contacts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ppo_contact_ppo_application_ppo_application_id",
                        column: x => x.ppo_application_id,
                        principalSchema: "cside",
                        principalTable: "ppo_application",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ppo_events",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ppo_application_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<Instant>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    event_date = table.Column<LocalDate>(type: "date", nullable: false),
                    event_text = table.Column<string>(type: "text", nullable: false),
                    author_id = table.Column<string>(type: "text", nullable: true),
                    author_name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ppo_events", x => x.id);
                    table.ForeignKey(
                        name: "fk_ppo_events_ppo_application_ppo_application_id",
                        column: x => x.ppo_application_id,
                        principalSchema: "cside",
                        principalTable: "ppo_application",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ppo_media",
                schema: "cside",
                columns: table => new
                {
                    ppo_id = table.Column<int>(type: "integer", nullable: false),
                    media_id = table.Column<int>(type: "integer", nullable: false),
                    media_type_id = table.Column<int>(type: "integer", nullable: false),
                    ppo_application_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ppo_media", x => new { x.ppo_id, x.media_id });
                    table.ForeignKey(
                        name: "fk_ppo_media_media_media_id",
                        column: x => x.media_id,
                        principalSchema: "cside",
                        principalTable: "media",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ppo_media_ppo_application_ppo_application_id",
                        column: x => x.ppo_application_id,
                        principalSchema: "cside",
                        principalTable: "ppo_application",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ppo_media_ppo_media_type_media_type_id",
                        column: x => x.media_type_id,
                        principalSchema: "cside",
                        principalTable: "ppo_media_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ppo_orders",
                schema: "cside",
                columns: table => new
                {
                    order_id = table.Column<int>(type: "integer", nullable: false),
                    ppo_application_id = table.Column<int>(type: "integer", nullable: false),
                    order_title = table.Column<string>(type: "text", nullable: true),
                    objections_end_date = table.Column<LocalDate>(type: "date", nullable: true),
                    objections_received = table.Column<bool>(type: "boolean", nullable: true),
                    objections_withdrawn = table.Column<bool>(type: "boolean", nullable: true),
                    determination_process_id = table.Column<int>(type: "integer", nullable: true),
                    decision_of_sec_state_id = table.Column<int>(type: "integer", nullable: true),
                    date_confirmed = table.Column<LocalDate>(type: "date", nullable: true),
                    date_sealed = table.Column<LocalDate>(type: "date", nullable: true),
                    date_published = table.Column<LocalDate>(type: "date", nullable: true),
                    submit_to_pins = table.Column<bool>(type: "boolean", nullable: true),
                    inspection_certification = table.Column<bool>(type: "boolean", nullable: true),
                    inspection_certification_date = table.Column<LocalDate>(type: "date", nullable: true),
                    confirmation_published_date = table.Column<LocalDate>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ppo_orders", x => new { x.order_id, x.ppo_application_id });
                    table.ForeignKey(
                        name: "fk_ppo_orders_order_decisions_of_sec_state_decision_of_sec_sta",
                        column: x => x.decision_of_sec_state_id,
                        principalSchema: "cside",
                        principalTable: "order_decisions_of_sec_state",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_ppo_orders_order_determination_processes_determination_proc",
                        column: x => x.determination_process_id,
                        principalSchema: "cside",
                        principalTable: "order_determination_processes",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_ppo_orders_ppo_application_ppo_application_id",
                        column: x => x.ppo_application_id,
                        principalSchema: "cside",
                        principalTable: "ppo_application",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ppo_parishes",
                schema: "cside",
                columns: table => new
                {
                    ppo_application_id = table.Column<int>(type: "integer", nullable: false),
                    parish_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ppo_parishes", x => new { x.ppo_application_id, x.parish_id });
                    table.ForeignKey(
                        name: "fk_ppo_parishes_parishes_parish_id",
                        column: x => x.parish_id,
                        principalSchema: "cside",
                        principalTable: "parishes",
                        principalColumn: "admin_unit_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ppo_parishes_ppo_application_ppo_application_id",
                        column: x => x.ppo_application_id,
                        principalSchema: "cside",
                        principalTable: "ppo_application",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ppo_types",
                schema: "cside",
                columns: table => new
                {
                    ppo_application_id = table.Column<int>(type: "integer", nullable: false),
                    type_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ppo_types", x => new { x.ppo_application_id, x.type_id });
                    table.ForeignKey(
                        name: "fk_ppo_types_ppo_application_ppo_application_id",
                        column: x => x.ppo_application_id,
                        principalSchema: "cside",
                        principalTable: "ppo_application",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ppo_types_ppo_application_types_type_id",
                        column: x => x.type_id,
                        principalSchema: "cside",
                        principalTable: "ppo_application_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "dmmo_linked_routes",
                schema: "cside",
                columns: table => new
                {
                    dmmo_application_id = table.Column<int>(type: "integer", nullable: false),
                    route_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dmmo_linked_routes", x => new { x.dmmo_application_id, x.route_id });
                    table.ForeignKey(
                        name: "fk_dmmo_linked_routes_dmmo_application_dmmo_application_id",
                        column: x => x.dmmo_application_id,
                        principalSchema: "cside",
                        principalTable: "dmmo_application",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_dmmo_linked_routes_routes_route_id",
                        column: x => x.route_id,
                        principalSchema: "cside",
                        principalTable: "routes",
                        principalColumn: "route_code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "infrastructure",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    installation_date = table.Column<LocalDate>(type: "date", nullable: true),
                    height = table.Column<double>(type: "double precision", nullable: true),
                    width = table.Column<double>(type: "double precision", nullable: true),
                    length = table.Column<double>(type: "double precision", nullable: true),
                    geom = table.Column<Point>(type: "geometry(point, 27700)", nullable: false),
                    route_id = table.Column<string>(type: "text", nullable: true),
                    infrastructure_type_id = table.Column<int>(type: "integer", nullable: true),
                    parish_id = table.Column<int>(type: "integer", nullable: true),
                    maintenance_team_id = table.Column<int>(type: "integer", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_infrastructure", x => x.id);
                    table.ForeignKey(
                        name: "fk_infrastructure_infrastructure_types_infrastructure_type_id",
                        column: x => x.infrastructure_type_id,
                        principalSchema: "cside",
                        principalTable: "infrastructure_types",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_infrastructure_maintenance_teams_maintenance_team_id",
                        column: x => x.maintenance_team_id,
                        principalSchema: "cside",
                        principalTable: "maintenance_teams",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_infrastructure_parishes_parish_id",
                        column: x => x.parish_id,
                        principalSchema: "cside",
                        principalTable: "parishes",
                        principalColumn: "admin_unit_id");
                    table.ForeignKey(
                        name: "fk_infrastructure_routes_route_id",
                        column: x => x.route_id,
                        principalSchema: "cside",
                        principalTable: "routes",
                        principalColumn: "route_code");
                });

            migrationBuilder.CreateTable(
                name: "maintenance_jobs",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    log_date = table.Column<Instant>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    problem_description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    redacted_problem_description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    completion_date = table.Column<LocalDate>(type: "date", nullable: true),
                    work_done = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    logged_by_id = table.Column<string>(type: "text", nullable: true),
                    logged_by_name = table.Column<string>(type: "text", nullable: true),
                    duplicate_job_id = table.Column<int>(type: "integer", nullable: true),
                    geom = table.Column<Point>(type: "geometry(point, 27700)", nullable: false),
                    job_status_id = table.Column<int>(type: "integer", nullable: false),
                    job_priority_id = table.Column<int>(type: "integer", nullable: false),
                    route_id = table.Column<string>(type: "text", nullable: true),
                    maintenance_team_id = table.Column<int>(type: "integer", nullable: true),
                    parish_id = table.Column<int>(type: "integer", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_maintenance_jobs", x => x.id);
                    table.ForeignKey(
                        name: "fk_maintenance_jobs_maintenance_job_priorities_job_priority_id",
                        column: x => x.job_priority_id,
                        principalSchema: "cside",
                        principalTable: "maintenance_job_priorities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_maintenance_jobs_maintenance_job_statuses_job_status_id",
                        column: x => x.job_status_id,
                        principalSchema: "cside",
                        principalTable: "maintenance_job_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_maintenance_jobs_maintenance_teams_maintenance_team_id",
                        column: x => x.maintenance_team_id,
                        principalSchema: "cside",
                        principalTable: "maintenance_teams",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_maintenance_jobs_parishes_parish_id",
                        column: x => x.parish_id,
                        principalSchema: "cside",
                        principalTable: "parishes",
                        principalColumn: "admin_unit_id");
                    table.ForeignKey(
                        name: "fk_maintenance_jobs_routes_route_id",
                        column: x => x.route_id,
                        principalSchema: "cside",
                        principalTable: "routes",
                        principalColumn: "route_code");
                });

            migrationBuilder.CreateTable(
                name: "route_media",
                schema: "cside",
                columns: table => new
                {
                    route_id = table.Column<string>(type: "text", nullable: false),
                    media_id = table.Column<int>(type: "integer", nullable: false),
                    is_closure_notification_document = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_route_media", x => new { x.route_id, x.media_id });
                    table.ForeignKey(
                        name: "fk_route_media_media_media_id",
                        column: x => x.media_id,
                        principalSchema: "cside",
                        principalTable: "media",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_route_media_routes_route_id",
                        column: x => x.route_id,
                        principalSchema: "cside",
                        principalTable: "routes",
                        principalColumn: "route_code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "statements",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    route_id = table.Column<string>(type: "text", nullable: false),
                    statement_text = table.Column<string>(type: "text", nullable: false),
                    start_grid_ref = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    end_grid_ref = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    version = table.Column<int>(type: "integer", nullable: false),
                    created_date = table.Column<Instant>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_statements", x => x.id);
                    table.ForeignKey(
                        name: "fk_statements_routes_route_id",
                        column: x => x.route_id,
                        principalSchema: "cside",
                        principalTable: "routes",
                        principalColumn: "route_code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "bridge_surveys",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('cside.\"SurveySequence\"')"),
                    start_date = table.Column<Instant>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    end_date = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<SurveyStatus>(type: "cside.survey_status", nullable: false, defaultValueSql: "'incomplete'::\"cside\".survey_status"),
                    surveyor_id = table.Column<string>(type: "text", nullable: true),
                    surveyor_name = table.Column<string>(type: "text", nullable: true),
                    repairs_required = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    validation_notes = table.Column<string>(type: "text", nullable: true),
                    infrastructure_item_id = table.Column<int>(type: "integer", nullable: false),
                    height = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    width = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    length = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    beam_condition_id = table.Column<int>(type: "integer", nullable: true),
                    decking_condition_id = table.Column<int>(type: "integer", nullable: true),
                    handrail_condition_id = table.Column<int>(type: "integer", nullable: true),
                    handrail_posts_condition_id = table.Column<int>(type: "integer", nullable: true),
                    bank_seat_condition_id = table.Column<int>(type: "integer", nullable: true),
                    beam_material_id = table.Column<int>(type: "integer", nullable: true),
                    decking_material_id = table.Column<int>(type: "integer", nullable: true),
                    handrail_material_id = table.Column<int>(type: "integer", nullable: true),
                    handrail_posts_material_id = table.Column<int>(type: "integer", nullable: true),
                    bank_seat_material_id = table.Column<int>(type: "integer", nullable: true),
                    num_beam_timbers = table.Column<int>(type: "integer", nullable: true),
                    num_decking_boards = table.Column<int>(type: "integer", nullable: true),
                    num_handrail_posts_timbers = table.Column<int>(type: "integer", nullable: true),
                    beam_timbers_size = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    decking_boards_size = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    decking_boards_length = table.Column<decimal>(type: "numeric(3,1)", precision: 3, scale: 1, nullable: true),
                    handrail_timbers_size = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    handrail_posts_timbers_size = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    handrails_in_place = table.Column<bool>(type: "boolean", nullable: true),
                    overgrown = table.Column<bool>(type: "boolean", nullable: true),
                    signs_of_bank_erosion = table.Column<bool>(type: "boolean", nullable: true),
                    seriously_eroded = table.Column<bool>(type: "boolean", nullable: true),
                    high_usage = table.Column<bool>(type: "boolean", nullable: true),
                    cover_boards_in_place = table.Column<bool>(type: "boolean", nullable: true),
                    ramp_installed = table.Column<bool>(type: "boolean", nullable: true),
                    steps_installed = table.Column<bool>(type: "boolean", nullable: true),
                    anti_slip_installed = table.Column<bool>(type: "boolean", nullable: true),
                    gate_installed = table.Column<bool>(type: "boolean", nullable: true),
                    stile_installed = table.Column<bool>(type: "boolean", nullable: true),
                    updated_x = table.Column<double>(type: "double precision", nullable: true),
                    updated_y = table.Column<double>(type: "double precision", nullable: true),
                    location_accuracy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bridge_surveys", x => x.id);
                    table.ForeignKey(
                        name: "fk_bridge_surveys_conditions_bank_seat_condition_id",
                        column: x => x.bank_seat_condition_id,
                        principalSchema: "cside",
                        principalTable: "conditions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_bridge_surveys_conditions_beam_condition_id",
                        column: x => x.beam_condition_id,
                        principalSchema: "cside",
                        principalTable: "conditions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_bridge_surveys_conditions_decking_condition_id",
                        column: x => x.decking_condition_id,
                        principalSchema: "cside",
                        principalTable: "conditions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_bridge_surveys_conditions_handrail_condition_id",
                        column: x => x.handrail_condition_id,
                        principalSchema: "cside",
                        principalTable: "conditions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_bridge_surveys_conditions_handrail_posts_condition_id",
                        column: x => x.handrail_posts_condition_id,
                        principalSchema: "cside",
                        principalTable: "conditions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_bridge_surveys_infrastructure_infrastructure_item_id",
                        column: x => x.infrastructure_item_id,
                        principalSchema: "cside",
                        principalTable: "infrastructure",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_bridge_surveys_materials_bank_seat_material_id",
                        column: x => x.bank_seat_material_id,
                        principalSchema: "cside",
                        principalTable: "materials",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_bridge_surveys_materials_beam_material_id",
                        column: x => x.beam_material_id,
                        principalSchema: "cside",
                        principalTable: "materials",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_bridge_surveys_materials_decking_material_id",
                        column: x => x.decking_material_id,
                        principalSchema: "cside",
                        principalTable: "materials",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_bridge_surveys_materials_handrail_material_id",
                        column: x => x.handrail_material_id,
                        principalSchema: "cside",
                        principalTable: "materials",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_bridge_surveys_materials_handrail_posts_material_id",
                        column: x => x.handrail_posts_material_id,
                        principalSchema: "cside",
                        principalTable: "materials",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "infrastructure_bridge_details",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    infrastructure_id = table.Column<int>(type: "integer", nullable: false),
                    beam_condition_id = table.Column<int>(type: "integer", nullable: true),
                    decking_condition_id = table.Column<int>(type: "integer", nullable: true),
                    handrail_condition_id = table.Column<int>(type: "integer", nullable: true),
                    handrail_posts_condition_id = table.Column<int>(type: "integer", nullable: true),
                    bank_seat_condition_id = table.Column<int>(type: "integer", nullable: true),
                    beam_material_id = table.Column<int>(type: "integer", nullable: true),
                    decking_material_id = table.Column<int>(type: "integer", nullable: true),
                    handrail_material_id = table.Column<int>(type: "integer", nullable: true),
                    handrail_posts_material_id = table.Column<int>(type: "integer", nullable: true),
                    bank_seat_material_id = table.Column<int>(type: "integer", nullable: true),
                    num_beam_timbers = table.Column<int>(type: "integer", nullable: true),
                    num_decking_boards = table.Column<int>(type: "integer", nullable: true),
                    num_handrail_posts_timbers = table.Column<int>(type: "integer", nullable: true),
                    beam_timbers_size = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    decking_boards_size = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    decking_boards_length = table.Column<decimal>(type: "numeric(3,1)", precision: 3, scale: 1, nullable: true),
                    handrail_timbers_size = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    handrail_posts_timbers_size = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    handrails_in_place = table.Column<bool>(type: "boolean", nullable: true),
                    overgrown = table.Column<bool>(type: "boolean", nullable: true),
                    signs_of_bank_erosion = table.Column<bool>(type: "boolean", nullable: true),
                    seriously_eroded = table.Column<bool>(type: "boolean", nullable: true),
                    high_usage = table.Column<bool>(type: "boolean", nullable: true),
                    cover_boards_in_place = table.Column<bool>(type: "boolean", nullable: true),
                    ramp_installed = table.Column<bool>(type: "boolean", nullable: true),
                    steps_installed = table.Column<bool>(type: "boolean", nullable: true),
                    anti_slip_installed = table.Column<bool>(type: "boolean", nullable: true),
                    gate_installed = table.Column<bool>(type: "boolean", nullable: true),
                    stile_installed = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_infrastructure_bridge_details", x => x.id);
                    table.ForeignKey(
                        name: "fk_infrastructure_bridge_details_conditions_bank_seat_conditio",
                        column: x => x.bank_seat_condition_id,
                        principalSchema: "cside",
                        principalTable: "conditions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_infrastructure_bridge_details_conditions_beam_condition_id",
                        column: x => x.beam_condition_id,
                        principalSchema: "cside",
                        principalTable: "conditions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_infrastructure_bridge_details_conditions_decking_condition_",
                        column: x => x.decking_condition_id,
                        principalSchema: "cside",
                        principalTable: "conditions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_infrastructure_bridge_details_conditions_handrail_condition",
                        column: x => x.handrail_condition_id,
                        principalSchema: "cside",
                        principalTable: "conditions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_infrastructure_bridge_details_conditions_handrail_posts_con",
                        column: x => x.handrail_posts_condition_id,
                        principalSchema: "cside",
                        principalTable: "conditions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_infrastructure_bridge_details_infrastructure_infrastructure",
                        column: x => x.infrastructure_id,
                        principalSchema: "cside",
                        principalTable: "infrastructure",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_infrastructure_bridge_details_materials_bank_seat_material_",
                        column: x => x.bank_seat_material_id,
                        principalSchema: "cside",
                        principalTable: "materials",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_infrastructure_bridge_details_materials_beam_material_id",
                        column: x => x.beam_material_id,
                        principalSchema: "cside",
                        principalTable: "materials",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_infrastructure_bridge_details_materials_decking_material_id",
                        column: x => x.decking_material_id,
                        principalSchema: "cside",
                        principalTable: "materials",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_infrastructure_bridge_details_materials_handrail_material_id",
                        column: x => x.handrail_material_id,
                        principalSchema: "cside",
                        principalTable: "materials",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_infrastructure_bridge_details_materials_handrail_posts_mate",
                        column: x => x.handrail_posts_material_id,
                        principalSchema: "cside",
                        principalTable: "materials",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "infrastructure_media",
                schema: "cside",
                columns: table => new
                {
                    infrastructure_item_id = table.Column<int>(type: "integer", nullable: false),
                    media_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_infrastructure_media", x => new { x.infrastructure_item_id, x.media_id });
                    table.ForeignKey(
                        name: "fk_infrastructure_media_infrastructure_infrastructure_item_id",
                        column: x => x.infrastructure_item_id,
                        principalSchema: "cside",
                        principalTable: "infrastructure",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_infrastructure_media_media_media_id",
                        column: x => x.media_id,
                        principalSchema: "cside",
                        principalTable: "media",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "maintenance_comments",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    job_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<Instant>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    comment_text = table.Column<string>(type: "text", nullable: false),
                    is_public = table.Column<bool>(type: "boolean", nullable: false),
                    author_id = table.Column<string>(type: "text", nullable: true),
                    author_name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_maintenance_comments", x => x.id);
                    table.ForeignKey(
                        name: "fk_maintenance_comments_maintenance_jobs_job_id",
                        column: x => x.job_id,
                        principalSchema: "cside",
                        principalTable: "maintenance_jobs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "maintenance_job_contact",
                schema: "cside",
                columns: table => new
                {
                    job_id = table.Column<int>(type: "integer", nullable: false),
                    contact_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_maintenance_job_contact", x => new { x.job_id, x.contact_id });
                    table.ForeignKey(
                        name: "fk_maintenance_job_contact_contacts_contact_id",
                        column: x => x.contact_id,
                        principalSchema: "cside",
                        principalTable: "contacts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_maintenance_job_contact_maintenance_jobs_job_id",
                        column: x => x.job_id,
                        principalSchema: "cside",
                        principalTable: "maintenance_jobs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "maintenance_job_infrastructure",
                schema: "cside",
                columns: table => new
                {
                    job_id = table.Column<int>(type: "integer", nullable: false),
                    infrastructure_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_maintenance_job_infrastructure", x => new { x.job_id, x.infrastructure_id });
                    table.ForeignKey(
                        name: "fk_maintenance_job_infrastructure_infrastructure_infrastructur",
                        column: x => x.infrastructure_id,
                        principalSchema: "cside",
                        principalTable: "infrastructure",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_maintenance_job_infrastructure_maintenance_jobs_job_id",
                        column: x => x.job_id,
                        principalSchema: "cside",
                        principalTable: "maintenance_jobs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "maintenance_job_media",
                schema: "cside",
                columns: table => new
                {
                    job_id = table.Column<int>(type: "integer", nullable: false),
                    media_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_maintenance_job_media", x => new { x.job_id, x.media_id });
                    table.ForeignKey(
                        name: "fk_maintenance_job_media_maintenance_jobs_job_id",
                        column: x => x.job_id,
                        principalSchema: "cside",
                        principalTable: "maintenance_jobs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_maintenance_job_media_media_media_id",
                        column: x => x.media_id,
                        principalSchema: "cside",
                        principalTable: "media",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "maintenance_job_problem_types",
                schema: "cside",
                columns: table => new
                {
                    job_id = table.Column<int>(type: "integer", nullable: false),
                    problem_type_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_maintenance_job_problem_types", x => new { x.job_id, x.problem_type_id });
                    table.ForeignKey(
                        name: "fk_maintenance_job_problem_types_maintenance_jobs_job_id",
                        column: x => x.job_id,
                        principalSchema: "cside",
                        principalTable: "maintenance_jobs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_maintenance_job_problem_types_problem_types_problem_type_id",
                        column: x => x.problem_type_id,
                        principalSchema: "cside",
                        principalTable: "problem_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "maintenance_job_subscribers",
                schema: "cside",
                columns: table => new
                {
                    job_id = table.Column<int>(type: "integer", nullable: false),
                    email_address = table.Column<string>(type: "text", nullable: false),
                    unsubscribe_token = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_maintenance_job_subscribers", x => new { x.job_id, x.email_address });
                    table.ForeignKey(
                        name: "fk_maintenance_job_subscribers_maintenance_jobs_job_id",
                        column: x => x.job_id,
                        principalSchema: "cside",
                        principalTable: "maintenance_jobs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "cside",
                table: "application_roles",
                columns: new[] { "id", "role_name" },
                values: new object[,]
                {
                    { 1, "Administrator" },
                    { 2, "Ranger" },
                    { 3, "RoW Officer" },
                    { 4, "Survey Validator" },
                    { 5, "RoW Statement Editor" },
                    { 6, "View" },
                    { 7, "Surveyor" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_application_user_roles_application_role_id",
                schema: "cside",
                table: "application_user_roles",
                column: "application_role_id");

            migrationBuilder.CreateIndex(
                name: "ix_bridge_surveys_bank_seat_condition_id",
                schema: "cside",
                table: "bridge_surveys",
                column: "bank_seat_condition_id");

            migrationBuilder.CreateIndex(
                name: "ix_bridge_surveys_bank_seat_material_id",
                schema: "cside",
                table: "bridge_surveys",
                column: "bank_seat_material_id");

            migrationBuilder.CreateIndex(
                name: "ix_bridge_surveys_beam_condition_id",
                schema: "cside",
                table: "bridge_surveys",
                column: "beam_condition_id");

            migrationBuilder.CreateIndex(
                name: "ix_bridge_surveys_beam_material_id",
                schema: "cside",
                table: "bridge_surveys",
                column: "beam_material_id");

            migrationBuilder.CreateIndex(
                name: "ix_bridge_surveys_decking_condition_id",
                schema: "cside",
                table: "bridge_surveys",
                column: "decking_condition_id");

            migrationBuilder.CreateIndex(
                name: "ix_bridge_surveys_decking_material_id",
                schema: "cside",
                table: "bridge_surveys",
                column: "decking_material_id");

            migrationBuilder.CreateIndex(
                name: "ix_bridge_surveys_handrail_condition_id",
                schema: "cside",
                table: "bridge_surveys",
                column: "handrail_condition_id");

            migrationBuilder.CreateIndex(
                name: "ix_bridge_surveys_handrail_material_id",
                schema: "cside",
                table: "bridge_surveys",
                column: "handrail_material_id");

            migrationBuilder.CreateIndex(
                name: "ix_bridge_surveys_handrail_posts_condition_id",
                schema: "cside",
                table: "bridge_surveys",
                column: "handrail_posts_condition_id");

            migrationBuilder.CreateIndex(
                name: "ix_bridge_surveys_handrail_posts_material_id",
                schema: "cside",
                table: "bridge_surveys",
                column: "handrail_posts_material_id");

            migrationBuilder.CreateIndex(
                name: "ix_bridge_surveys_infrastructure_item_id",
                schema: "cside",
                table: "bridge_surveys",
                column: "infrastructure_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_contacts_contact_type_id",
                schema: "cside",
                table: "contacts",
                column: "contact_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_dmmo_application_case_status_id",
                schema: "cside",
                table: "dmmo_application",
                column: "case_status_id");

            migrationBuilder.CreateIndex(
                name: "ix_dmmo_application_direction_of_sec_state_id",
                schema: "cside",
                table: "dmmo_application",
                column: "direction_of_sec_state_id");

            migrationBuilder.CreateIndex(
                name: "ix_dmmo_claimed_statuses_claimed_status_id",
                schema: "cside",
                table: "dmmo_claimed_statuses",
                column: "claimed_status_id");

            migrationBuilder.CreateIndex(
                name: "ix_dmmo_contact_contact_id",
                schema: "cside",
                table: "dmmo_contact",
                column: "contact_id");

            migrationBuilder.CreateIndex(
                name: "ix_dmmo_events_dmmo_application_id",
                schema: "cside",
                table: "dmmo_events",
                column: "dmmo_application_id");

            migrationBuilder.CreateIndex(
                name: "ix_dmmo_linked_routes_route_id",
                schema: "cside",
                table: "dmmo_linked_routes",
                column: "route_id");

            migrationBuilder.CreateIndex(
                name: "ix_dmmo_media_dmmo_application_id",
                schema: "cside",
                table: "dmmo_media",
                column: "dmmo_application_id");

            migrationBuilder.CreateIndex(
                name: "ix_dmmo_media_media_id",
                schema: "cside",
                table: "dmmo_media",
                column: "media_id");

            migrationBuilder.CreateIndex(
                name: "ix_dmmo_media_media_type_id",
                schema: "cside",
                table: "dmmo_media",
                column: "media_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_dmmo_orders_decision_of_sec_state_id",
                schema: "cside",
                table: "dmmo_orders",
                column: "decision_of_sec_state_id");

            migrationBuilder.CreateIndex(
                name: "ix_dmmo_orders_determination_process_id",
                schema: "cside",
                table: "dmmo_orders",
                column: "determination_process_id");

            migrationBuilder.CreateIndex(
                name: "ix_dmmo_orders_dmmo_application_id",
                schema: "cside",
                table: "dmmo_orders",
                column: "dmmo_application_id");

            migrationBuilder.CreateIndex(
                name: "ix_dmmo_parish_parish_id",
                schema: "cside",
                table: "dmmo_parish",
                column: "parish_id");

            migrationBuilder.CreateIndex(
                name: "ix_dmmo_types_application_type_id",
                schema: "cside",
                table: "dmmo_types",
                column: "application_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_infrastructure_infrastructure_type_id",
                schema: "cside",
                table: "infrastructure",
                column: "infrastructure_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_infrastructure_maintenance_team_id",
                schema: "cside",
                table: "infrastructure",
                column: "maintenance_team_id");

            migrationBuilder.CreateIndex(
                name: "ix_infrastructure_parish_id",
                schema: "cside",
                table: "infrastructure",
                column: "parish_id");

            migrationBuilder.CreateIndex(
                name: "ix_infrastructure_route_id",
                schema: "cside",
                table: "infrastructure",
                column: "route_id");

            migrationBuilder.CreateIndex(
                name: "ix_infrastructure_bridge_details_bank_seat_condition_id",
                schema: "cside",
                table: "infrastructure_bridge_details",
                column: "bank_seat_condition_id");

            migrationBuilder.CreateIndex(
                name: "ix_infrastructure_bridge_details_bank_seat_material_id",
                schema: "cside",
                table: "infrastructure_bridge_details",
                column: "bank_seat_material_id");

            migrationBuilder.CreateIndex(
                name: "ix_infrastructure_bridge_details_beam_condition_id",
                schema: "cside",
                table: "infrastructure_bridge_details",
                column: "beam_condition_id");

            migrationBuilder.CreateIndex(
                name: "ix_infrastructure_bridge_details_beam_material_id",
                schema: "cside",
                table: "infrastructure_bridge_details",
                column: "beam_material_id");

            migrationBuilder.CreateIndex(
                name: "ix_infrastructure_bridge_details_decking_condition_id",
                schema: "cside",
                table: "infrastructure_bridge_details",
                column: "decking_condition_id");

            migrationBuilder.CreateIndex(
                name: "ix_infrastructure_bridge_details_decking_material_id",
                schema: "cside",
                table: "infrastructure_bridge_details",
                column: "decking_material_id");

            migrationBuilder.CreateIndex(
                name: "ix_infrastructure_bridge_details_handrail_condition_id",
                schema: "cside",
                table: "infrastructure_bridge_details",
                column: "handrail_condition_id");

            migrationBuilder.CreateIndex(
                name: "ix_infrastructure_bridge_details_handrail_material_id",
                schema: "cside",
                table: "infrastructure_bridge_details",
                column: "handrail_material_id");

            migrationBuilder.CreateIndex(
                name: "ix_infrastructure_bridge_details_handrail_posts_condition_id",
                schema: "cside",
                table: "infrastructure_bridge_details",
                column: "handrail_posts_condition_id");

            migrationBuilder.CreateIndex(
                name: "ix_infrastructure_bridge_details_handrail_posts_material_id",
                schema: "cside",
                table: "infrastructure_bridge_details",
                column: "handrail_posts_material_id");

            migrationBuilder.CreateIndex(
                name: "ix_infrastructure_bridge_details_infrastructure_id",
                schema: "cside",
                table: "infrastructure_bridge_details",
                column: "infrastructure_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_infrastructure_media_media_id",
                schema: "cside",
                table: "infrastructure_media",
                column: "media_id");

            migrationBuilder.CreateIndex(
                name: "ix_landowner_deposit_contacts_contact_id",
                schema: "cside",
                table: "landowner_deposit_contacts",
                column: "contact_id");

            migrationBuilder.CreateIndex(
                name: "ix_landowner_deposit_events_landowner_deposit_id_landowner_dep",
                schema: "cside",
                table: "landowner_deposit_events",
                columns: new[] { "landowner_deposit_id", "landowner_deposit_secondary_id" });

            migrationBuilder.CreateIndex(
                name: "ix_landowner_deposit_media_media_id",
                schema: "cside",
                table: "landowner_deposit_media",
                column: "media_id");

            migrationBuilder.CreateIndex(
                name: "ix_landowner_deposit_media_media_type_id",
                schema: "cside",
                table: "landowner_deposit_media",
                column: "media_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_landowner_deposit_parishes_parish_id",
                schema: "cside",
                table: "landowner_deposit_parishes",
                column: "parish_id");

            migrationBuilder.CreateIndex(
                name: "ix_landowner_deposit_types_landowner_deposit_type_name_id",
                schema: "cside",
                table: "landowner_deposit_types",
                column: "landowner_deposit_type_name_id");

            migrationBuilder.CreateIndex(
                name: "ix_maintenance_comments_job_id",
                schema: "cside",
                table: "maintenance_comments",
                column: "job_id");

            migrationBuilder.CreateIndex(
                name: "ix_maintenance_job_contact_contact_id",
                schema: "cside",
                table: "maintenance_job_contact",
                column: "contact_id");

            migrationBuilder.CreateIndex(
                name: "ix_maintenance_job_infrastructure_infrastructure_id",
                schema: "cside",
                table: "maintenance_job_infrastructure",
                column: "infrastructure_id");

            migrationBuilder.CreateIndex(
                name: "ix_maintenance_job_media_media_id",
                schema: "cside",
                table: "maintenance_job_media",
                column: "media_id");

            migrationBuilder.CreateIndex(
                name: "ix_maintenance_job_problem_types_problem_type_id",
                schema: "cside",
                table: "maintenance_job_problem_types",
                column: "problem_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_maintenance_jobs_job_priority_id",
                schema: "cside",
                table: "maintenance_jobs",
                column: "job_priority_id");

            migrationBuilder.CreateIndex(
                name: "ix_maintenance_jobs_job_status_id",
                schema: "cside",
                table: "maintenance_jobs",
                column: "job_status_id");

            migrationBuilder.CreateIndex(
                name: "ix_maintenance_jobs_maintenance_team_id",
                schema: "cside",
                table: "maintenance_jobs",
                column: "maintenance_team_id");

            migrationBuilder.CreateIndex(
                name: "ix_maintenance_jobs_parish_id",
                schema: "cside",
                table: "maintenance_jobs",
                column: "parish_id");

            migrationBuilder.CreateIndex(
                name: "ix_maintenance_jobs_route_id",
                schema: "cside",
                table: "maintenance_jobs",
                column: "route_id");

            migrationBuilder.CreateIndex(
                name: "ix_ppo_application_case_status_id",
                schema: "cside",
                table: "ppo_application",
                column: "case_status_id");

            migrationBuilder.CreateIndex(
                name: "ix_ppo_application_legislation_id",
                schema: "cside",
                table: "ppo_application",
                column: "legislation_id");

            migrationBuilder.CreateIndex(
                name: "ix_ppo_application_priority_id",
                schema: "cside",
                table: "ppo_application",
                column: "priority_id");

            migrationBuilder.CreateIndex(
                name: "ix_ppo_contact_contact_id",
                schema: "cside",
                table: "ppo_contact",
                column: "contact_id");

            migrationBuilder.CreateIndex(
                name: "ix_ppo_events_ppo_application_id",
                schema: "cside",
                table: "ppo_events",
                column: "ppo_application_id");

            migrationBuilder.CreateIndex(
                name: "ix_ppo_media_media_id",
                schema: "cside",
                table: "ppo_media",
                column: "media_id");

            migrationBuilder.CreateIndex(
                name: "ix_ppo_media_media_type_id",
                schema: "cside",
                table: "ppo_media",
                column: "media_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_ppo_media_ppo_application_id",
                schema: "cside",
                table: "ppo_media",
                column: "ppo_application_id");

            migrationBuilder.CreateIndex(
                name: "ix_ppo_orders_decision_of_sec_state_id",
                schema: "cside",
                table: "ppo_orders",
                column: "decision_of_sec_state_id");

            migrationBuilder.CreateIndex(
                name: "ix_ppo_orders_determination_process_id",
                schema: "cside",
                table: "ppo_orders",
                column: "determination_process_id");

            migrationBuilder.CreateIndex(
                name: "ix_ppo_orders_ppo_application_id",
                schema: "cside",
                table: "ppo_orders",
                column: "ppo_application_id");

            migrationBuilder.CreateIndex(
                name: "ix_ppo_parishes_parish_id",
                schema: "cside",
                table: "ppo_parishes",
                column: "parish_id");

            migrationBuilder.CreateIndex(
                name: "ix_ppo_types_type_id",
                schema: "cside",
                table: "ppo_types",
                column: "type_id");

            migrationBuilder.CreateIndex(
                name: "ix_route_media_media_id",
                schema: "cside",
                table: "route_media",
                column: "media_id");

            migrationBuilder.CreateIndex(
                name: "ix_routes_legal_status_id",
                schema: "cside",
                table: "routes",
                column: "legal_status_id");

            migrationBuilder.CreateIndex(
                name: "ix_routes_maintenance_team_id",
                schema: "cside",
                table: "routes",
                column: "maintenance_team_id");

            migrationBuilder.CreateIndex(
                name: "ix_routes_operational_status_id",
                schema: "cside",
                table: "routes",
                column: "operational_status_id");

            migrationBuilder.CreateIndex(
                name: "ix_routes_parish_id",
                schema: "cside",
                table: "routes",
                column: "parish_id");

            migrationBuilder.CreateIndex(
                name: "ix_routes_route_type_id",
                schema: "cside",
                table: "routes",
                column: "route_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_statements_route_id",
                schema: "cside",
                table: "statements",
                column: "route_id");

            migrationBuilder.CreateIndex(
                name: "ix_survey_media_media_id",
                schema: "cside",
                table: "survey_media",
                column: "media_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "application_user_roles",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "audit_logs",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "bridge_surveys",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "dmmo_addresses",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "dmmo_claimed_statuses",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "dmmo_contact",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "dmmo_events",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "dmmo_linked_routes",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "dmmo_media",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "dmmo_orders",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "dmmo_parish",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "dmmo_types",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "infrastructure_bridge_details",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "infrastructure_media",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "landowner_deposit_addresses",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "landowner_deposit_contacts",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "landowner_deposit_events",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "landowner_deposit_media",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "landowner_deposit_parishes",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "landowner_deposit_types",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "maintenance_comments",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "maintenance_job_contact",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "maintenance_job_infrastructure",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "maintenance_job_media",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "maintenance_job_problem_types",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "maintenance_job_subscribers",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "maintenance_team_users",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "parish_codes",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "ppo_contact",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "ppo_events",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "ppo_media",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "ppo_orders",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "ppo_parishes",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "ppo_types",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "route_media",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "statements",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "survey_media",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "application_roles",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "dmmo_application_claimed_statuses",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "dmmo_media_type",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "dmmo_application",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "dmmo_application_types",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "conditions",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "materials",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "landowner_deposit_media_types",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "landowner_deposit_type_names",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "landowner_deposits",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "infrastructure",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "problem_types",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "maintenance_jobs",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "contacts",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "ppo_media_type",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "order_decisions_of_sec_state",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "order_determination_processes",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "ppo_application",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "ppo_application_types",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "media",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "dmmo_application_case_statuses",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "dmmo_application_directions_of_sec_state",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "infrastructure_types",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "maintenance_job_priorities",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "maintenance_job_statuses",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "routes",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "contact_types",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "ppo_application_case_statuses",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "ppo_application_legislation",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "ppo_application_priorities",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "maintenance_teams",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "route_legal_statuses",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "route_operational_statuses",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "route_types",
                schema: "cside");

            migrationBuilder.DropSequence(
                name: "SurveySequence",
                schema: "cside");
        }
    }
}
