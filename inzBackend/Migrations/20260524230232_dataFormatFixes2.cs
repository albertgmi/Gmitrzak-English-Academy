using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inzBackend.Migrations
{
    /// <inheritdoc />
    public partial class dataFormatFixes2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
