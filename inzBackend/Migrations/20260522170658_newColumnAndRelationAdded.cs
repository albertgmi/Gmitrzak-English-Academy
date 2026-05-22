using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inzBackend.Migrations
{
    /// <inheritdoc />
    public partial class newColumnAndRelationAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CatalogueId",
                table: "Vocabulary",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 22, 17, 6, 57, 98, DateTimeKind.Unspecified).AddTicks(8434), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 22, 17, 6, 57, 98, DateTimeKind.Unspecified).AddTicks(8443), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEPK4i3erd1fDuYP2N20e/yUJRrtLfRRfiMyav/iZHy4PI9lQ1CfCztnS8EEA2xDmPQ==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 22, 17, 6, 57, 147, DateTimeKind.Unspecified).AddTicks(5869), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 22, 17, 6, 57, 147, DateTimeKind.Unspecified).AddTicks(5878), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEJYmPrwqJYFT2flS5BW1RmLazsTqDIbxB+DUvGOnpaIcH45RYl/fvGPXCHsMIxff/Q==" });

            migrationBuilder.CreateIndex(
                name: "IX_Vocabulary_CatalogueId",
                table: "Vocabulary",
                column: "CatalogueId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vocabulary_Catalogues_CatalogueId",
                table: "Vocabulary",
                column: "CatalogueId",
                principalTable: "Catalogues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vocabulary_Catalogues_CatalogueId",
                table: "Vocabulary");

            migrationBuilder.DropIndex(
                name: "IX_Vocabulary_CatalogueId",
                table: "Vocabulary");

            migrationBuilder.DropColumn(
                name: "CatalogueId",
                table: "Vocabulary");

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
    }
}
