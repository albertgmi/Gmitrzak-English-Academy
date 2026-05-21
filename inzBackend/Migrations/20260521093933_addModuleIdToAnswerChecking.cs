using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inzBackend.Migrations
{
    /// <inheritdoc />
    public partial class addModuleIdToAnswerChecking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSentenceAnswers_UserSentenceAssignments_AssignmentId",
                table: "UserSentenceAnswers");

            migrationBuilder.RenameColumn(
                name: "AssignmentId",
                table: "UserSentenceAnswers",
                newName: "ModuleId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSentenceAnswers_AssignmentId",
                table: "UserSentenceAnswers",
                newName: "IX_UserSentenceAnswers_ModuleId");

            migrationBuilder.AddColumn<int>(
                name: "UserSentenceAssignmentId",
                table: "UserSentenceAnswers",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 21, 9, 39, 31, 441, DateTimeKind.Unspecified).AddTicks(9443), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 21, 9, 39, 31, 441, DateTimeKind.Unspecified).AddTicks(9454), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEKW8/pBfobn1g4rm4cHfjsrSl3R1evZjgDuIHsxrocfcMrGAhbL9JczeIKBqx/kn5g==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 21, 9, 39, 31, 571, DateTimeKind.Unspecified).AddTicks(2811), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 21, 9, 39, 31, 571, DateTimeKind.Unspecified).AddTicks(2820), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEL8skPsj8eC6+r16oGzh72JODua7kt9waRmmtpr9yNTfhxPk0DjkHkPGfxk5ba7WOw==" });

            migrationBuilder.CreateIndex(
                name: "IX_UserSentenceAnswers_UserSentenceAssignmentId",
                table: "UserSentenceAnswers",
                column: "UserSentenceAssignmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSentenceAnswers_Modules_ModuleId",
                table: "UserSentenceAnswers",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSentenceAnswers_UserSentenceAssignments_UserSentenceAss~",
                table: "UserSentenceAnswers",
                column: "UserSentenceAssignmentId",
                principalTable: "UserSentenceAssignments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSentenceAnswers_Modules_ModuleId",
                table: "UserSentenceAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSentenceAnswers_UserSentenceAssignments_UserSentenceAss~",
                table: "UserSentenceAnswers");

            migrationBuilder.DropIndex(
                name: "IX_UserSentenceAnswers_UserSentenceAssignmentId",
                table: "UserSentenceAnswers");

            migrationBuilder.DropColumn(
                name: "UserSentenceAssignmentId",
                table: "UserSentenceAnswers");

            migrationBuilder.RenameColumn(
                name: "ModuleId",
                table: "UserSentenceAnswers",
                newName: "AssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSentenceAnswers_ModuleId",
                table: "UserSentenceAnswers",
                newName: "IX_UserSentenceAnswers_AssignmentId");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 21, 9, 2, 14, 615, DateTimeKind.Unspecified).AddTicks(6570), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 21, 9, 2, 14, 615, DateTimeKind.Unspecified).AddTicks(6580), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEL/lkPGMNlrnOKTWtueeaw2CsRENqmFucnz8or4/btBP6k4VEj7l9qxF0J74P3oCng==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 21, 9, 2, 14, 726, DateTimeKind.Unspecified).AddTicks(691), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 21, 9, 2, 14, 726, DateTimeKind.Unspecified).AddTicks(698), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEDiJ+RjBHwo+UnvHNbjPXH9GSx/QXOcI2avKc4AlY+XaNXQsAazF6VIM6Mn70Mo2ww==" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserSentenceAnswers_UserSentenceAssignments_AssignmentId",
                table: "UserSentenceAnswers",
                column: "AssignmentId",
                principalTable: "UserSentenceAssignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
