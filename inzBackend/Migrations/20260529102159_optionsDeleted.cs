using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace inzBackend.Migrations
{
    /// <inheritdoc />
    public partial class optionsDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModulePresentations_Modules_ModuleId1",
                table: "ModulePresentations");

            migrationBuilder.DropTable(
                name: "UserOptions");

            migrationBuilder.DropIndex(
                name: "IX_ModulePresentations_ModuleId1",
                table: "ModulePresentations");

            migrationBuilder.DropColumn(
                name: "ModuleId1",
                table: "ModulePresentations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ModuleId1",
                table: "ModulePresentations",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    EmailNotifications = table.Column<bool>(type: "boolean", nullable: false),
                    IncorrectStepFourMinutes = table.Column<int>(type: "integer", nullable: false),
                    IncorrectStepOneMinutes = table.Column<int>(type: "integer", nullable: false),
                    IncorrectStepThreeMinutes = table.Column<int>(type: "integer", nullable: false),
                    IncorrectStepTwoMinutes = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    LastModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LeechThreshold = table.Column<int>(type: "integer", nullable: false),
                    MinDailyFlashcards = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserOptions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModulePresentations_ModuleId1",
                table: "ModulePresentations",
                column: "ModuleId1",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserOptions_UserId",
                table: "UserOptions",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ModulePresentations_Modules_ModuleId1",
                table: "ModulePresentations",
                column: "ModuleId1",
                principalTable: "Modules",
                principalColumn: "Id");
        }
    }
}
