using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inzBackend.Migrations
{
    /// <inheritdoc />
    public partial class nullableAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatrixModules_Matrices_MatrixId1",
                table: "MatrixModules");

            migrationBuilder.DropIndex(
                name: "IX_MatrixModules_MatrixId1",
                table: "MatrixModules");

            migrationBuilder.DropColumn(
                name: "MatrixId1",
                table: "MatrixModules");

            migrationBuilder.AlterColumn<bool>(
                name: "IsHidden",
                table: "Modules",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Modules",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsHidden",
                table: "Modules",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Modules",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MatrixId1",
                table: "MatrixModules",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MatrixModules_MatrixId1",
                table: "MatrixModules",
                column: "MatrixId1");

            migrationBuilder.AddForeignKey(
                name: "FK_MatrixModules_Matrices_MatrixId1",
                table: "MatrixModules",
                column: "MatrixId1",
                principalTable: "Matrices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
