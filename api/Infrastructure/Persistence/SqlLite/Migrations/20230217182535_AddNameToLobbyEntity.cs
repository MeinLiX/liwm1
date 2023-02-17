using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    /// <inheritdoc />
    public partial class AddNameToLobbyEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LobbyCreatorId",
                table: "Lobbies",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LobbyName",
                table: "Lobbies",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Lobbies_LobbyCreatorId",
                table: "Lobbies",
                column: "LobbyCreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lobbies_AspNetUsers_LobbyCreatorId",
                table: "Lobbies",
                column: "LobbyCreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lobbies_AspNetUsers_LobbyCreatorId",
                table: "Lobbies");

            migrationBuilder.DropIndex(
                name: "IX_Lobbies_LobbyCreatorId",
                table: "Lobbies");

            migrationBuilder.DropColumn(
                name: "LobbyCreatorId",
                table: "Lobbies");

            migrationBuilder.DropColumn(
                name: "LobbyName",
                table: "Lobbies");
        }
    }
}
