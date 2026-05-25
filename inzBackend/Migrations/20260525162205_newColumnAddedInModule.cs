using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inzBackend.Migrations
{
    /// <inheritdoc />
    public partial class newColumnAddedInModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TheaterItemId",
                table: "Modules",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 25, 18, 22, 4, 97, DateTimeKind.Unspecified).AddTicks(9390), new TimeSpan(0, 2, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 25, 18, 22, 4, 97, DateTimeKind.Unspecified).AddTicks(9509), new TimeSpan(0, 2, 0, 0, 0)), "AQAAAAIAAYagAAAAEMlclJXaNEPY7Tdrq32B09Q3yTNiI5kooab2Ygo4wmPVWPRlgPODxhiW5j0xAys8QA==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 25, 18, 22, 4, 145, DateTimeKind.Unspecified).AddTicks(3025), new TimeSpan(0, 2, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 25, 18, 22, 4, 145, DateTimeKind.Unspecified).AddTicks(3201), new TimeSpan(0, 2, 0, 0, 0)), "AQAAAAIAAYagAAAAEMz8L7/LgnPetUumO0FWnrYPtIM48NZyrRvSKuHu/7RE2M5YgsaP3O4i6mcLqzbuPQ==" });

            migrationBuilder.CreateIndex(
                name: "IX_Modules_TheaterItemId",
                table: "Modules",
                column: "TheaterItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Modules_TheaterItems_TheaterItemId",
                table: "Modules",
                column: "TheaterItemId",
                principalTable: "TheaterItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Modules_TheaterItems_TheaterItemId",
                table: "Modules");

            migrationBuilder.DropIndex(
                name: "IX_Modules_TheaterItemId",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "TheaterItemId",
                table: "Modules");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 25, 1, 2, 30, 932, DateTimeKind.Unspecified).AddTicks(9980), new TimeSpan(0, 2, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 25, 1, 2, 30, 933, DateTimeKind.Unspecified).AddTicks(87), new TimeSpan(0, 2, 0, 0, 0)), "AQAAAAIAAYagAAAAEJMvtzKeki+JaENiVDj5HCRFRVHHLAZXppDtHtAOjp9f8JBlpQSwolqIty8vO3/v2w==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 25, 1, 2, 30, 983, DateTimeKind.Unspecified).AddTicks(926), new TimeSpan(0, 2, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 25, 1, 2, 30, 983, DateTimeKind.Unspecified).AddTicks(1120), new TimeSpan(0, 2, 0, 0, 0)), "AQAAAAIAAYagAAAAEMIZoBhcVaTI3wWotks9R4LZ6Xg5H5xgbbgv1iCysMqhpPLc9wNz+mViITJ74LY8IQ==" });
        }
    }
}
