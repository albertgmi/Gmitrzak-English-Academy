using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inzBackend.Migrations
{
    /// <inheritdoc />
    public partial class teacherExplanationAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TeacherExplanation",
                table: "UserSentenceAnswers",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 21, 20, 56, 17, 862, DateTimeKind.Unspecified).AddTicks(9897), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 21, 20, 56, 17, 862, DateTimeKind.Unspecified).AddTicks(9903), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEEYi3xg4SZ6gL6yoKG0bGB5LbKL812zSidyZW0WYxU1nv9kbPoowe6NsF38fauTGAw==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 21, 20, 56, 17, 934, DateTimeKind.Unspecified).AddTicks(4268), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 21, 20, 56, 17, 934, DateTimeKind.Unspecified).AddTicks(4275), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEFW2Skta+gSHN0XAHfktybcn5HRHs+GxKFp+Bv6OYcWq9Dhl7ckGFnU+XMdQ8xN0lA==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TeacherExplanation",
                table: "UserSentenceAnswers");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 21, 18, 44, 3, 202, DateTimeKind.Unspecified).AddTicks(5499), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 21, 18, 44, 3, 202, DateTimeKind.Unspecified).AddTicks(5512), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEBs1dh3/dJCgIJZtBGlx6dk6EMSS2owoItkBzQF+Ya5jaTFzqsC3EU+NzBC50Rifrw==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 21, 18, 44, 3, 249, DateTimeKind.Unspecified).AddTicks(9429), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 21, 18, 44, 3, 249, DateTimeKind.Unspecified).AddTicks(9435), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAELNGdKSY19GiJ0lyLKb/KNJ9dl8HvXvGgAZ3p43rOMotrMOkbsCs/vqU0yDqenup4Q==" });
        }
    }
}
