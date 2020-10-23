using Microsoft.EntityFrameworkCore.Migrations;

namespace QuestBoard.Migrations
{
    public partial class addnametoboard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boards_AspNetUsers_BoardOwnerEmail",
                table: "Boards");

            migrationBuilder.DropIndex(
                name: "IX_Boards_BoardOwnerEmail",
                table: "Boards");

            migrationBuilder.DropColumn(
                name: "BoardOwnerEmail",
                table: "Boards");

            migrationBuilder.AddColumn<string>(
                name: "BoardName",
                table: "Boards",
                nullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boards_AspNetUsers_BoardName",
                table: "Boards");

            migrationBuilder.DropIndex(
                name: "IX_Boards_BoardName",
                table: "Boards");

            migrationBuilder.DropColumn(
                name: "BoardName",
                table: "Boards");

            migrationBuilder.AddColumn<string>(
                name: "BoardOwnerEmail",
                table: "Boards",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Boards_BoardOwnerEmail",
                table: "Boards",
                column: "BoardOwnerEmail",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Boards_AspNetUsers_BoardOwnerEmail",
                table: "Boards",
                column: "BoardOwnerEmail",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
