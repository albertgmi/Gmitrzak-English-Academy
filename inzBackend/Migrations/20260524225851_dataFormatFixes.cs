using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inzBackend.Migrations
{
    /// <inheritdoc />
    public partial class dataFormatFixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 25, 0, 58, 50, 252, DateTimeKind.Unspecified).AddTicks(8678), new TimeSpan(0, 2, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 25, 0, 58, 50, 252, DateTimeKind.Unspecified).AddTicks(8756), new TimeSpan(0, 2, 0, 0, 0)), "AQAAAAIAAYagAAAAEF/uBDkeqzn5etOOwFkSDRK+VVAjUuXRpyXU+FZBbN8aQhkDwiSW47Fn/hdG+xNrLw==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 25, 0, 58, 50, 302, DateTimeKind.Unspecified).AddTicks(8699), new TimeSpan(0, 2, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 25, 0, 58, 50, 302, DateTimeKind.Unspecified).AddTicks(8808), new TimeSpan(0, 2, 0, 0, 0)), "AQAAAAIAAYagAAAAEFiSK71qpH6tiXnzKo+NHPfetn3NxFn38g5B581TUdZEeOWvY7tzwOuhKFbYHCy6lg==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 25, 0, 43, 46, 252, DateTimeKind.Unspecified).AddTicks(2018), new TimeSpan(0, 2, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 25, 0, 43, 46, 252, DateTimeKind.Unspecified).AddTicks(2112), new TimeSpan(0, 2, 0, 0, 0)), "AQAAAAIAAYagAAAAEH11HCgXd9LX1sazo9Ki6lJ7A1BvyrO7aAjDlM4SJmWgsyey+XpONoZSLiJxdp9FZw==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 25, 0, 43, 46, 299, DateTimeKind.Unspecified).AddTicks(9057), new TimeSpan(0, 2, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 25, 0, 43, 46, 299, DateTimeKind.Unspecified).AddTicks(9148), new TimeSpan(0, 2, 0, 0, 0)), "AQAAAAIAAYagAAAAEERVmdXePhLc/u8J7IadPLp/Sy/5DP1+4vle81V7oa1XTyFwBiEcwebBPPm3EPqPKg==" });
        }
    }
}
