using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inzBackend.Migrations
{
    /// <inheritdoc />
    public partial class newColumnAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReviewed",
                table: "UserSentenceAssignments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReviewed",
                table: "Sentences",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 20, 20, 52, 17, 228, DateTimeKind.Unspecified).AddTicks(1488), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 20, 20, 52, 17, 228, DateTimeKind.Unspecified).AddTicks(1496), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAED3DzL+wRfubYCEnJNtrlgZhDUnm7/l6iel21vc9tDeNu3nTJUCQoxhImofMG0de8w==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 20, 20, 52, 17, 276, DateTimeKind.Unspecified).AddTicks(5797), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 20, 20, 52, 17, 276, DateTimeKind.Unspecified).AddTicks(5805), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEOAUK2hq+mI5DQWxwvFENI/k3AWIFFH/O01nMGJ1m2ZydUJhhakCeBjcnDt8CDErUw==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReviewed",
                table: "UserSentenceAssignments");

            migrationBuilder.DropColumn(
                name: "IsReviewed",
                table: "Sentences");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 20, 20, 0, 11, 288, DateTimeKind.Unspecified).AddTicks(1390), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 20, 20, 0, 11, 288, DateTimeKind.Unspecified).AddTicks(1396), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEIufEDEcS/BIGnXlkS5MW+bnsI9F57mJOEM+wHxGouIt7pAWFU6EdezptKnffSQOzg==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 20, 20, 0, 11, 335, DateTimeKind.Unspecified).AddTicks(3187), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 20, 20, 0, 11, 335, DateTimeKind.Unspecified).AddTicks(3194), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAELV2jGyr7KHrsK+dglDgx4i6xHahBSKyoFRdu/EC1wMu0/QHBrUMCu8eaBGMNgbPMw==" });
        }
    }
}
