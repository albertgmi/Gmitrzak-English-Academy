using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inzBackend.Migrations
{
    /// <inheritdoc />
    public partial class translatedColumnForAi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 19, 13, 43, 57, 975, DateTimeKind.Unspecified).AddTicks(4207), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 19, 13, 43, 57, 975, DateTimeKind.Unspecified).AddTicks(4219), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEBuwunyPHw44xJWI7MlhLYK77eS4grOAHwGDFsSAG1Unhf6vFMiGCJIh0qpMdHkQPg==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 19, 13, 43, 58, 22, DateTimeKind.Unspecified).AddTicks(9634), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 19, 13, 43, 58, 22, DateTimeKind.Unspecified).AddTicks(9644), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEMJ27rzivGtdUhucy4yiAlWgY2qYqWcFlpnmLKorqh/FE7pZkYrNq00NbnXwj+SFew==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 18, 11, 19, 0, 101, DateTimeKind.Unspecified).AddTicks(100), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 18, 11, 19, 0, 101, DateTimeKind.Unspecified).AddTicks(105), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEAolv2b4zHM7GZ/o01f80KGjGOg/tiKIw9QrB7fHjFU1PNqRXQn92kF8yLyULvRNmg==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 18, 11, 19, 0, 148, DateTimeKind.Unspecified).AddTicks(9189), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 18, 11, 19, 0, 148, DateTimeKind.Unspecified).AddTicks(9196), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEDRQYoD4QjZKzKBnQpzA+D0YvSMkRIsIBwUjdo1dlB4+ndTMj7F9bTY/kFXuxCidCw==" });
        }
    }
}
