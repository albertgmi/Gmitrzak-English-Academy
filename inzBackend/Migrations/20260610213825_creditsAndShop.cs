using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace inzBackend.Migrations
{
    /// <inheritdoc />
    public partial class creditsAndShop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Credits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Credits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Credits_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShopItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CreditCost = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IconEmoji = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShopPurchases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ShopItemId = table.Column<int>(type: "integer", nullable: false),
                    CreditCost = table.Column<int>(type: "integer", nullable: false),
                    PurchaseDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopPurchases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopPurchases_ShopItems_ShopItemId",
                        column: x => x.ShopItemId,
                        principalTable: "ShopItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShopPurchases_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ShopItems",
                columns: new[] { "Id", "CreatedAt", "CreditCost", "Description", "IconEmoji", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, new DateTimeOffset(new DateTime(2026, 6, 10, 23, 38, 22, 821, DateTimeKind.Unspecified).AddTicks(5609), new TimeSpan(0, 2, 0, 0, 0)), 10, "Lorem ipsum dolor sit amet, skip one assignment without penalty.", "📝", true, "Skip homework pass" },
                    { 2, new DateTimeOffset(new DateTime(2026, 6, 10, 23, 38, 22, 821, DateTimeKind.Unspecified).AddTicks(5717), new TimeSpan(0, 2, 0, 0, 0)), 20, "Lorem ipsum consectetur, get an extra 30-minute conversation session.", "🎓", true, "Bonus lesson" },
                    { 3, new DateTimeOffset(new DateTime(2026, 6, 10, 23, 38, 22, 821, DateTimeKind.Unspecified).AddTicks(5721), new TimeSpan(0, 2, 0, 0, 0)), 15, "Lorem ipsum adipiscing, request a custom flashcard set on any topic.", "🃏", true, "Custom flashcard pack" },
                    { 4, new DateTimeOffset(new DateTime(2026, 6, 10, 23, 38, 22, 821, DateTimeKind.Unspecified).AddTicks(5725), new TimeSpan(0, 2, 0, 0, 0)), 25, "Lorem ipsum elit, dedicated 45-minute grammar troubleshooting session.", "📚", true, "Grammar deep-dive" },
                    { 5, new DateTimeOffset(new DateTime(2026, 6, 10, 23, 38, 22, 821, DateTimeKind.Unspecified).AddTicks(5728), new TimeSpan(0, 2, 0, 0, 0)), 30, "Lorem ipsum sed, full pronunciation review with personalized feedback.", "🎤", true, "Pronunciation audit" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Credits_UserId",
                table: "Credits",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopPurchases_ShopItemId",
                table: "ShopPurchases",
                column: "ShopItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopPurchases_UserId",
                table: "ShopPurchases",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Credits");

            migrationBuilder.DropTable(
                name: "ShopPurchases");

            migrationBuilder.DropTable(
                name: "ShopItems");
        }
    }
}
