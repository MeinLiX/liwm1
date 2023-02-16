using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    /// <inheritdoc />
    public partial class AddLobbyEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndGame",
                table: "Games",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LobbyId",
                table: "Games",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartGame",
                table: "Games",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "LobbyId",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Lobbies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lobbies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_LobbyId",
                table: "Games",
                column: "LobbyId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LobbyId",
                table: "AspNetUsers",
                column: "LobbyId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Lobbies_LobbyId",
                table: "AspNetUsers",
                column: "LobbyId",
                principalTable: "Lobbies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Lobbies_LobbyId",
                table: "Games",
                column: "LobbyId",
                principalTable: "Lobbies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Lobbies_LobbyId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_Lobbies_LobbyId",
                table: "Games");

            migrationBuilder.DropTable(
                name: "Lobbies");

            migrationBuilder.DropIndex(
                name: "IX_Games_LobbyId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LobbyId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EndGame",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "LobbyId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "StartGame",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "LobbyId",
                table: "AspNetUsers");
        }
    }
}
