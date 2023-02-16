using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    /// <inheritdoc />
    public partial class AddGameModeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.AddColumn<int>(
                name: "GameModeId",
                table: "Lobbies",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GameModes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PreviewUrl = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameModes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lobbies_GameModeId",
                table: "Lobbies",
                column: "GameModeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lobbies_GameModes_GameModeId",
                table: "Lobbies",
                column: "GameModeId",
                principalTable: "GameModes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lobbies_GameModes_GameModeId",
                table: "Lobbies");

            migrationBuilder.DropTable(
                name: "GameModes");

            migrationBuilder.DropIndex(
                name: "IX_Lobbies_GameModeId",
                table: "Lobbies");

            migrationBuilder.DropColumn(
                name: "GameModeId",
                table: "Lobbies");

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EndGame = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LobbyId = table.Column<int>(type: "INTEGER", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    PreviewUrl = table.Column<string>(type: "TEXT", nullable: false),
                    StartGame = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_Lobbies_LobbyId",
                        column: x => x.LobbyId,
                        principalTable: "Lobbies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_LobbyId",
                table: "Games",
                column: "LobbyId");
        }
    }
}
