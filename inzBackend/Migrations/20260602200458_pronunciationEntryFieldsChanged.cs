using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inzBackend.Migrations
{
    /// <inheritdoc />
    public partial class pronunciationEntryFieldsChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsChecked",
                table: "PronunciationEntries",
                newName: "IsInCurrentSession");

            migrationBuilder.AddColumn<DateOnly>(
                name: "MarkedCorrectAt",
                table: "PronunciationEntries",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "PronunciationEntries",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MarkedCorrectAt",
                table: "PronunciationEntries");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "PronunciationEntries");

            migrationBuilder.RenameColumn(
                name: "IsInCurrentSession",
                table: "PronunciationEntries",
                newName: "IsChecked");
        }
    }
}
