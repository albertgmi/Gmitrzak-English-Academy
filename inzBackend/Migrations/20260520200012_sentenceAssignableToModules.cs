using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace inzBackend.Migrations
{
    /// <inheritdoc />
    public partial class sentenceAssignableToModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModuleSentenceSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ModuleId = table.Column<int>(type: "integer", nullable: false),
                    SentenceSetId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleSentenceSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModuleSentenceSets_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModuleSentenceSets_SentenceSets_SentenceSetId",
                        column: x => x.SentenceSetId,
                        principalTable: "SentenceSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 20, 20, 0, 11, 288, DateTimeKind.Unspecified).AddTicks(1390), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 20, 20, 0, 11, 288, DateTimeKind.Unspecified).AddTicks(1396), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEIufEDEcS/BIGnXlkS5MW+bnsI9F57mJOEM+wHxGouIt7pAWFU6EdezptKnffSQOzg==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 20, 20, 0, 11, 335, DateTimeKind.Unspecified).AddTicks(3187), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 20, 20, 0, 11, 335, DateTimeKind.Unspecified).AddTicks(3194), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAELV2jGyr7KHrsK+dglDgx4i6xHahBSKyoFRdu/EC1wMu0/QHBrUMCu8eaBGMNgbPMw==" });

            migrationBuilder.CreateIndex(
                name: "IX_ModuleSentenceSets_ModuleId",
                table: "ModuleSentenceSets",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleSentenceSets_SentenceSetId",
                table: "ModuleSentenceSets",
                column: "SentenceSetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModuleSentenceSets");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 19, 16, 39, 11, 256, DateTimeKind.Unspecified).AddTicks(8929), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 19, 16, 39, 11, 256, DateTimeKind.Unspecified).AddTicks(8940), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEGTzyriupNIxgr93gTn+Fkuox7i66PDy7SWGFNop/e9MmmKN+d7DNDG3mz6dNXUwHg==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 19, 16, 39, 11, 316, DateTimeKind.Unspecified).AddTicks(238), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 19, 16, 39, 11, 316, DateTimeKind.Unspecified).AddTicks(246), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEGZSvS3ePRiYGazO5rVN9MX//fg7kQ8PK2pl285SOr61uyfZ7xL0aBs0p0oRsa5UTA==" });
        }
    }
}
