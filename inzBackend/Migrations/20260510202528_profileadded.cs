using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace inzBackend.Migrations
{
    /// <inheritdoc />
    public partial class profileadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    AvatarUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    EnglishLevel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CurrentSemester = table.Column<int>(type: "integer", nullable: true, defaultValue: 1),
                    Semester1 = table.Column<bool>(type: "boolean", nullable: false),
                    Semester2 = table.Column<bool>(type: "boolean", nullable: false),
                    Semester3 = table.Column<bool>(type: "boolean", nullable: false),
                    Semester4 = table.Column<bool>(type: "boolean", nullable: false),
                    Semester5 = table.Column<bool>(type: "boolean", nullable: false),
                    Semester6 = table.Column<bool>(type: "boolean", nullable: false),
                    Semester7 = table.Column<bool>(type: "boolean", nullable: false),
                    Semester8 = table.Column<bool>(type: "boolean", nullable: false),
                    Semester9 = table.Column<bool>(type: "boolean", nullable: false),
                    Semester10 = table.Column<bool>(type: "boolean", nullable: false),
                    Semester11 = table.Column<bool>(type: "boolean", nullable: false),
                    Semester12 = table.Column<bool>(type: "boolean", nullable: false),
                    Semester13 = table.Column<bool>(type: "boolean", nullable: false),
                    Semester14 = table.Column<bool>(type: "boolean", nullable: false),
                    Semester15 = table.Column<bool>(type: "boolean", nullable: false),
                    Semester16 = table.Column<bool>(type: "boolean", nullable: false),
                    Semester17 = table.Column<bool>(type: "boolean", nullable: false),
                    Semester18 = table.Column<bool>(type: "boolean", nullable: false),
                    Semester19 = table.Column<bool>(type: "boolean", nullable: false),
                    Semester20 = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Profiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_UserId",
                table: "Profiles",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Profiles");
        }
    }
}
