using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace inzBackend.Migrations
{
    /// <inheritdoc />
    public partial class moduleupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Order",
                table: "MatrixModules",
                newName: "WeekNumber");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "MatrixModules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DayOfWeek",
                table: "MatrixModules",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "MatrixModules",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastModifiedAt",
                table: "MatrixModules",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "MatrixModules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MatrixId1",
                table: "MatrixModules",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UserModuleAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ModuleId = table.Column<int>(type: "integer", nullable: false),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserModuleAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserModuleAssignments_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserModuleAssignments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MatrixModules_MatrixId1",
                table: "MatrixModules",
                column: "MatrixId1");

            migrationBuilder.CreateIndex(
                name: "IX_UserModuleAssignments_ModuleId",
                table: "UserModuleAssignments",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserModuleAssignments_UserId",
                table: "UserModuleAssignments",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MatrixModules_Matrices_MatrixId1",
                table: "MatrixModules",
                column: "MatrixId1",
                principalTable: "Matrices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatrixModules_Matrices_MatrixId1",
                table: "MatrixModules");

            migrationBuilder.DropTable(
                name: "UserModuleAssignments");

            migrationBuilder.DropIndex(
                name: "IX_MatrixModules_MatrixId1",
                table: "MatrixModules");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "MatrixModules");

            migrationBuilder.DropColumn(
                name: "DayOfWeek",
                table: "MatrixModules");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "MatrixModules");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                table: "MatrixModules");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "MatrixModules");

            migrationBuilder.DropColumn(
                name: "MatrixId1",
                table: "MatrixModules");

            migrationBuilder.RenameColumn(
                name: "WeekNumber",
                table: "MatrixModules",
                newName: "Order");
        }
    }
}
