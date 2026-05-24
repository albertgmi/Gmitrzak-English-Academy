using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inzBackend.Migrations
{
    /// <inheritdoc />
    public partial class dataUtcDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
