using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace inzBackend.Migrations
{
    /// <inheritdoc />
    public partial class reactionTableAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 99);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 100);

            migrationBuilder.CreateTable(
                name: "RankingReactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FromUserId = table.Column<int>(type: "integer", nullable: false),
                    ToUserId = table.Column<int>(type: "integer", nullable: false),
                    Emoji = table.Column<string>(type: "text", nullable: false),
                    Period = table.Column<string>(type: "text", nullable: false),
                    ReactionDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RankingReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RankingReactions_Users_FromUserId",
                        column: x => x.FromUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RankingReactions_Users_ToUserId",
                        column: x => x.ToUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RankingReactions_FromUserId",
                table: "RankingReactions",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RankingReactions_ToUserId",
                table: "RankingReactions",
                column: "ToUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RankingReactions");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Email", "IsActive", "IsDeleted", "LastModifiedAt", "LastModifiedBy", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { 99, new DateTimeOffset(new DateTime(2026, 5, 25, 21, 47, 2, 554, DateTimeKind.Unspecified).AddTicks(687), new TimeSpan(0, 2, 0, 0, 0)), "System", "admin@example.com", true, false, new DateTimeOffset(new DateTime(2026, 5, 25, 21, 47, 2, 554, DateTimeKind.Unspecified).AddTicks(820), new TimeSpan(0, 2, 0, 0, 0)), "System", "AQAAAAIAAYagAAAAEPnymj0v6jmg3dOlbWa+9V1eQ/MpCbNqC6vM0qjlyWqZpyj+Pn0iFolvm7GqhtjARw==", "Admin", "testadmin" },
                    { 100, new DateTimeOffset(new DateTime(2026, 5, 25, 21, 47, 2, 602, DateTimeKind.Unspecified).AddTicks(244), new TimeSpan(0, 2, 0, 0, 0)), "System", "user@example.com", true, false, new DateTimeOffset(new DateTime(2026, 5, 25, 21, 47, 2, 602, DateTimeKind.Unspecified).AddTicks(374), new TimeSpan(0, 2, 0, 0, 0)), "System", "AQAAAAIAAYagAAAAEGcYTRDl5RVPsX+X9kVP/0xpCHXcz9ekP+iBdW8zagZsjdEzQ7zTaEdTdKl4kU/GJA==", "User", "testauser" }
                });
        }
    }
}
