using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inzBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddUserModuleAssignmentIdToUserEssay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserModuleAssignmentId",
                table: "UserEssays",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 9, 5, 10, 52, 6, 386, DateTimeKind.Unspecified).AddTicks(6469), new TimeSpan(0, 2, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 9, 5, 10, 52, 6, 386, DateTimeKind.Unspecified).AddTicks(6579), new TimeSpan(0, 2, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 9, 5, 10, 52, 6, 386, DateTimeKind.Unspecified).AddTicks(6584), new TimeSpan(0, 2, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 9, 5, 10, 52, 6, 386, DateTimeKind.Unspecified).AddTicks(6588), new TimeSpan(0, 2, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 9, 5, 10, 52, 6, 386, DateTimeKind.Unspecified).AddTicks(6591), new TimeSpan(0, 2, 0, 0, 0)));

            migrationBuilder.CreateIndex(
                name: "IX_UserEssays_UserModuleAssignmentId",
                table: "UserEssays",
                column: "UserModuleAssignmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserEssays_UserModuleAssignments_UserModuleAssignmentId",
                table: "UserEssays",
                column: "UserModuleAssignmentId",
                principalTable: "UserModuleAssignments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserEssays_UserModuleAssignments_UserModuleAssignmentId",
                table: "UserEssays");

            migrationBuilder.DropIndex(
                name: "IX_UserEssays_UserModuleAssignmentId",
                table: "UserEssays");

            migrationBuilder.DropColumn(
                name: "UserModuleAssignmentId",
                table: "UserEssays");

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 9, 5, 10, 51, 18, 997, DateTimeKind.Unspecified).AddTicks(6264), new TimeSpan(0, 2, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 9, 5, 10, 51, 18, 997, DateTimeKind.Unspecified).AddTicks(6436), new TimeSpan(0, 2, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 9, 5, 10, 51, 18, 997, DateTimeKind.Unspecified).AddTicks(6441), new TimeSpan(0, 2, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 9, 5, 10, 51, 18, 997, DateTimeKind.Unspecified).AddTicks(6455), new TimeSpan(0, 2, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 9, 5, 10, 51, 18, 997, DateTimeKind.Unspecified).AddTicks(6463), new TimeSpan(0, 2, 0, 0, 0)));
        }
    }
}
