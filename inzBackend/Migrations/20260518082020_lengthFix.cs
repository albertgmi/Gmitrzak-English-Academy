using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inzBackend.Migrations
{
    /// <inheritdoc />
    public partial class lengthFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Level",
                table: "TheaterItems",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 18, 8, 20, 19, 570, DateTimeKind.Unspecified).AddTicks(6373), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 18, 8, 20, 19, 570, DateTimeKind.Unspecified).AddTicks(6382), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEI4IS770fo/a8emdS9FgCre2rdbNaF+Xp3WHmiKXGoaZJDKUfvUOozkRvZLSGAMWKw==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 18, 8, 20, 19, 621, DateTimeKind.Unspecified).AddTicks(4240), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 18, 8, 20, 19, 621, DateTimeKind.Unspecified).AddTicks(4253), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEFwjWSR/s4rbc1cRt2Xb93OsLHeVcE6jC3T5UTQWDYSifFcE4MOsRD4oWul8jLQGqg==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Level",
                table: "TheaterItems",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 18, 8, 1, 29, 233, DateTimeKind.Unspecified).AddTicks(7658), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 18, 8, 1, 29, 233, DateTimeKind.Unspecified).AddTicks(7666), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEFq4Gy4W182STDHQY76Rk41N9z+rO2gC3LE9bLLZtZ5smjz2Tag11CIGEFYzkyyymg==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 18, 8, 1, 29, 283, DateTimeKind.Unspecified).AddTicks(5271), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 18, 8, 1, 29, 283, DateTimeKind.Unspecified).AddTicks(5285), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEIVOg6WCVR7IS5RIaAmGNcozW1KMV/FKzLucG+HWjBeIgy94ffZjyZtUUvi/eFX10Q==" });
        }
    }
}
