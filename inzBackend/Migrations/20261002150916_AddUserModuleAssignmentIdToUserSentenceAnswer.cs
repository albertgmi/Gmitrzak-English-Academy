using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inzBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddUserModuleAssignmentIdToUserSentenceAnswer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserModuleAssignmentId",
                table: "UserSentenceAnswers",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 10, 2, 17, 9, 15, 437, DateTimeKind.Unspecified).AddTicks(6327), new TimeSpan(0, 2, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 10, 2, 17, 9, 15, 437, DateTimeKind.Unspecified).AddTicks(6495), new TimeSpan(0, 2, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 10, 2, 17, 9, 15, 437, DateTimeKind.Unspecified).AddTicks(6500), new TimeSpan(0, 2, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 10, 2, 17, 9, 15, 437, DateTimeKind.Unspecified).AddTicks(6514), new TimeSpan(0, 2, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 10, 2, 17, 9, 15, 437, DateTimeKind.Unspecified).AddTicks(6523), new TimeSpan(0, 2, 0, 0, 0)));

            migrationBuilder.CreateIndex(
                name: "IX_UserSentenceAnswers_UserModuleAssignmentId",
                table: "UserSentenceAnswers",
                column: "UserModuleAssignmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSentenceAnswers_UserModuleAssignments_UserModuleAssignm~",
                table: "UserSentenceAnswers",
                column: "UserModuleAssignmentId",
                principalTable: "UserModuleAssignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSentenceAnswers_UserModuleAssignments_UserModuleAssignm~",
                table: "UserSentenceAnswers");

            migrationBuilder.DropIndex(
                name: "IX_UserSentenceAnswers_UserModuleAssignmentId",
                table: "UserSentenceAnswers");

            migrationBuilder.DropColumn(
                name: "UserModuleAssignmentId",
                table: "UserSentenceAnswers");

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 9, 28, 21, 25, 15, 203, DateTimeKind.Unspecified).AddTicks(2628), new TimeSpan(0, 2, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 9, 28, 21, 25, 15, 203, DateTimeKind.Unspecified).AddTicks(2740), new TimeSpan(0, 2, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 9, 28, 21, 25, 15, 203, DateTimeKind.Unspecified).AddTicks(2744), new TimeSpan(0, 2, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 9, 28, 21, 25, 15, 203, DateTimeKind.Unspecified).AddTicks(2755), new TimeSpan(0, 2, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 9, 28, 21, 25, 15, 203, DateTimeKind.Unspecified).AddTicks(2763), new TimeSpan(0, 2, 0, 0, 0)));
        }
    }
}
