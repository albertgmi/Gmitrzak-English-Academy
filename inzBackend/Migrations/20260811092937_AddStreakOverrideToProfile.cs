using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inzBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddStreakOverrideToProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StreakOverride",
                table: "Profiles",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 8, 11, 11, 29, 36, 405, DateTimeKind.Unspecified).AddTicks(8233), new TimeSpan(0, 2, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 8, 11, 11, 29, 36, 405, DateTimeKind.Unspecified).AddTicks(8348), new TimeSpan(0, 2, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 8, 11, 11, 29, 36, 405, DateTimeKind.Unspecified).AddTicks(8353), new TimeSpan(0, 2, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 8, 11, 11, 29, 36, 405, DateTimeKind.Unspecified).AddTicks(8363), new TimeSpan(0, 2, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 8, 11, 11, 29, 36, 405, DateTimeKind.Unspecified).AddTicks(8370), new TimeSpan(0, 2, 0, 0, 0)));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StreakOverride",
                table: "Profiles");

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 8, 7, 20, 31, 34, 524, DateTimeKind.Unspecified).AddTicks(8085), new TimeSpan(0, 2, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 8, 7, 20, 31, 34, 524, DateTimeKind.Unspecified).AddTicks(8190), new TimeSpan(0, 2, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 8, 7, 20, 31, 34, 524, DateTimeKind.Unspecified).AddTicks(8194), new TimeSpan(0, 2, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 8, 7, 20, 31, 34, 524, DateTimeKind.Unspecified).AddTicks(8198), new TimeSpan(0, 2, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 8, 7, 20, 31, 34, 524, DateTimeKind.Unspecified).AddTicks(8201), new TimeSpan(0, 2, 0, 0, 0)));
        }
    }
}
