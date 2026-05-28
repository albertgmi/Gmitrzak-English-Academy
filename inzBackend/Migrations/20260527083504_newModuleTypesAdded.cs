using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace inzBackend.Migrations
{
    /// <inheritdoc />
    public partial class newModuleTypesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModulePresentations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ModuleId = table.Column<int>(type: "integer", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: true),
                    Text = table.Column<string>(type: "text", nullable: true),
                    ModuleId1 = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModulePresentations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModulePresentations_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModulePresentations_Modules_ModuleId1",
                        column: x => x.ModuleId1,
                        principalTable: "Modules",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SectionActivityLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Section = table.Column<string>(type: "text", nullable: false),
                    ActivityDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionActivityLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SectionActivityLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModulePresentations_ModuleId",
                table: "ModulePresentations",
                column: "ModuleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModulePresentations_ModuleId1",
                table: "ModulePresentations",
                column: "ModuleId1",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SectionActivityLogs_UserId_Section_ActivityDate",
                table: "SectionActivityLogs",
                columns: new[] { "UserId", "Section", "ActivityDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModulePresentations");

            migrationBuilder.DropTable(
                name: "SectionActivityLogs");
        }
    }
}
