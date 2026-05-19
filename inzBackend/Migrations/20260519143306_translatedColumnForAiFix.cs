using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inzBackend.Migrations
{
    /// <inheritdoc />
    public partial class translatedColumnForAiFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TranslatedEntry",
                table: "CatalogueEntries",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 19, 14, 33, 4, 919, DateTimeKind.Unspecified).AddTicks(3395), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 19, 14, 33, 4, 919, DateTimeKind.Unspecified).AddTicks(3401), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEDfzi/dcUbMAvmQHZmIkowtSGngUAYNKwIsu6hsl0VmAaeTNwuChoWZZZyGs30lK+w==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 19, 14, 33, 4, 967, DateTimeKind.Unspecified).AddTicks(2767), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 19, 14, 33, 4, 967, DateTimeKind.Unspecified).AddTicks(2779), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEEVldkrXtSxsgAivtvOFcFz01IgSFxXlnllZI1Qk1PohFN1Wuxk3GPHA7cgkWtYgVQ==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TranslatedEntry",
                table: "CatalogueEntries");

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
    }
}
