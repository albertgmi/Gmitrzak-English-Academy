using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace inzBackend.Migrations
{
    /// <inheritdoc />
    public partial class theaterItemAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TheaterItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    MediaType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TheaterItems", x => x.Id);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TheaterItems");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 17, 23, 5, 37, 726, DateTimeKind.Unspecified).AddTicks(2571), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 17, 23, 5, 37, 726, DateTimeKind.Unspecified).AddTicks(2575), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEJFAyZz0g1JRHR4ozPGi1SLFOTDRiyMNHLIu/PCe73Xa1plNzoSDuAhb72UOGJm6JA==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 17, 23, 5, 37, 773, DateTimeKind.Unspecified).AddTicks(698), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 17, 23, 5, 37, 773, DateTimeKind.Unspecified).AddTicks(704), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEFvafTidp1vVnMliV/pADVrR7B6zRcuRUOHw+zuGvzxwhWAXxhHi9NNQX/GgpKdP+w==" });
        }
    }
}
