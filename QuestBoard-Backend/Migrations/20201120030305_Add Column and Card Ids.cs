using Microsoft.EntityFrameworkCore.Migrations;

namespace QuestBoard.Migrations
{
    public partial class AddColumnandCardIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_Columns_ColumnID",
                table: "Cards");

            migrationBuilder.DropForeignKey(
                name: "FK_Columns_Boards_BoardId",
                table: "Columns");

            migrationBuilder.RenameColumn(
                name: "ColumnID",
                table: "Cards",
                newName: "ColumnId");

            migrationBuilder.RenameIndex(
                name: "IX_Cards_ColumnID",
                table: "Cards",
                newName: "IX_Cards_ColumnId");

            migrationBuilder.AlterColumn<int>(
                name: "BoardId",
                table: "Columns",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ColumnId",
                table: "Cards",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_Columns_ColumnId",
                table: "Cards",
                column: "ColumnId",
                principalTable: "Columns",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Columns_Boards_BoardId",
                table: "Columns",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_Columns_ColumnId",
                table: "Cards");

            migrationBuilder.DropForeignKey(
                name: "FK_Columns_Boards_BoardId",
                table: "Columns");

            migrationBuilder.RenameColumn(
                name: "ColumnId",
                table: "Cards",
                newName: "ColumnID");

            migrationBuilder.RenameIndex(
                name: "IX_Cards_ColumnId",
                table: "Cards",
                newName: "IX_Cards_ColumnID");

            migrationBuilder.AlterColumn<int>(
                name: "BoardId",
                table: "Columns",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "ColumnID",
                table: "Cards",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_Columns_ColumnID",
                table: "Cards",
                column: "ColumnID",
                principalTable: "Columns",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Columns_Boards_BoardId",
                table: "Columns",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
