using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inzBackend.Migrations
{
    /// <inheritdoc />
    public partial class announcementsNewColumnsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "PronunciationEntries",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Announcements",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "SignedUp",
                table: "AnnouncementRecipients",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Vote",
                table: "AnnouncementRecipients",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "SignedUp",
                table: "AnnouncementRecipients");

            migrationBuilder.DropColumn(
                name: "Vote",
                table: "AnnouncementRecipients");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "PronunciationEntries",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
