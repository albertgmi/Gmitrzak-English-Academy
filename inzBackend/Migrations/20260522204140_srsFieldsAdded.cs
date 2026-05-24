using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inzBackend.Migrations
{
    /// <inheritdoc />
    public partial class srsFieldsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EaseFactor",
                table: "Sentences",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Interval",
                table: "Sentences",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsLeech",
                table: "Sentences",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateOnly>(
                name: "NextReviewDate",
                table: "Sentences",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 22, 20, 41, 39, 525, DateTimeKind.Unspecified).AddTicks(7804), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 22, 20, 41, 39, 525, DateTimeKind.Unspecified).AddTicks(7809), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEDoFbLwXIP/JZhFI5Fj+GIq7CCRqdUb/nJQxOyeE9vbLCotTbPglwOPkZmnwM3FivA==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 22, 20, 41, 39, 579, DateTimeKind.Unspecified).AddTicks(5002), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 22, 20, 41, 39, 579, DateTimeKind.Unspecified).AddTicks(5012), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEJ8Vd1gOTxutlDMa0es/mEanXLgbjijXByPc3yxIY0kJlU6ys9ijDF/DXOkvXlohkw==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EaseFactor",
                table: "Sentences");

            migrationBuilder.DropColumn(
                name: "Interval",
                table: "Sentences");

            migrationBuilder.DropColumn(
                name: "IsLeech",
                table: "Sentences");

            migrationBuilder.DropColumn(
                name: "NextReviewDate",
                table: "Sentences");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 22, 17, 6, 57, 98, DateTimeKind.Unspecified).AddTicks(8434), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 22, 17, 6, 57, 98, DateTimeKind.Unspecified).AddTicks(8443), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEPK4i3erd1fDuYP2N20e/yUJRrtLfRRfiMyav/iZHy4PI9lQ1CfCztnS8EEA2xDmPQ==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 22, 17, 6, 57, 147, DateTimeKind.Unspecified).AddTicks(5869), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 22, 17, 6, 57, 147, DateTimeKind.Unspecified).AddTicks(5878), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEJYmPrwqJYFT2flS5BW1RmLazsTqDIbxB+DUvGOnpaIcH45RYl/fvGPXCHsMIxff/Q==" });
        }
    }
}
