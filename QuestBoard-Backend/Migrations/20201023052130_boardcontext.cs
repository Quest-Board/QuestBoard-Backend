using Microsoft.EntityFrameworkCore.Migrations;

namespace QuestBoard.Migrations
{
    public partial class boardcontext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boards_AspNetUsers_BoardName",
                table: "Boards");

            migrationBuilder.DropIndex(
                name: "IX_Boards_BoardName",
                table: "Boards");

            migrationBuilder.AddColumn<string>(
                name: "BoardOwnerId",
                table: "Boards",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Boards_BoardOwnerId",
                table: "Boards",
                column: "BoardOwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Boards_AspNetUsers_BoardOwnerId",
                table: "Boards",
                column: "BoardOwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boards_AspNetUsers_BoardOwnerId",
                table: "Boards");

            migrationBuilder.DropIndex(
                name: "IX_Boards_BoardOwnerId",
                table: "Boards");

            migrationBuilder.DropColumn(
                name: "BoardOwnerId",
                table: "Boards");

            migrationBuilder.CreateIndex(
                name: "IX_Boards_BoardName",
                table: "Boards",
                column: "BoardName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Boards_AspNetUsers_BoardName",
                table: "Boards",
                column: "BoardName",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
