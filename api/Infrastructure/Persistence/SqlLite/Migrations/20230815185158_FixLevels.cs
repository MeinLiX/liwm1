using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    /// <inheritdoc />
    public partial class FixLevels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_GameId",
                table: "AspNetUsers",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Games_GameId",
                table: "AspNetUsers",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Games_GameId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_GameId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "AspNetUsers");
        }
    }
}
