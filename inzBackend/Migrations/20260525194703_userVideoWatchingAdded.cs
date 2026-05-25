using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inzBackend.Migrations
{
    /// <inheritdoc />
    public partial class userVideoWatchingAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 25, 21, 47, 2, 554, DateTimeKind.Unspecified).AddTicks(687), new TimeSpan(0, 2, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 25, 21, 47, 2, 554, DateTimeKind.Unspecified).AddTicks(820), new TimeSpan(0, 2, 0, 0, 0)), "AQAAAAIAAYagAAAAEPnymj0v6jmg3dOlbWa+9V1eQ/MpCbNqC6vM0qjlyWqZpyj+Pn0iFolvm7GqhtjARw==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 25, 21, 47, 2, 602, DateTimeKind.Unspecified).AddTicks(244), new TimeSpan(0, 2, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 25, 21, 47, 2, 602, DateTimeKind.Unspecified).AddTicks(374), new TimeSpan(0, 2, 0, 0, 0)), "AQAAAAIAAYagAAAAEGcYTRDl5RVPsX+X9kVP/0xpCHXcz9ekP+iBdW8zagZsjdEzQ7zTaEdTdKl4kU/GJA==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
