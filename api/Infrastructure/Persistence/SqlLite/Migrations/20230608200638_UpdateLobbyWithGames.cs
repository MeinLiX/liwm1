using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    /// <inheritdoc />
    public partial class UpdateLobbyWithGames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Games_GameId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_GameModes_GameModeId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_GameId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "GameState",
                table: "Games",
                newName: "State");

            migrationBuilder.RenameColumn(
                name: "GameModeId",
                table: "Games",
                newName: "ModeId");

            migrationBuilder.RenameIndex(
                name: "IX_Games_GameModeId",
                table: "Games",
                newName: "IX_Games_ModeId");

            migrationBuilder.AddColumn<bool>(
                name: "IsFinished",
                table: "RacingCars",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LobbyId",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LobbyId1",
                table: "Games",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GameAppUsersStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    AppUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Place = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameAppUsersStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameAppUsersStats_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameAppUsersStats_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_LobbyId",
                table: "Games",
                column: "LobbyId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_LobbyId1",
                table: "Games",
                column: "LobbyId1",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameAppUsersStats_AppUserId",
                table: "GameAppUsersStats",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GameAppUsersStats_GameId",
                table: "GameAppUsersStats",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_GameModes_ModeId",
                table: "Games",
                column: "ModeId",
                principalTable: "GameModes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Lobbies_LobbyId",
                table: "Games",
                column: "LobbyId",
                principalTable: "Lobbies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Lobbies_LobbyId1",
                table: "Games",
                column: "LobbyId1",
                principalTable: "Lobbies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_GameModes_ModeId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_Lobbies_LobbyId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_Lobbies_LobbyId1",
                table: "Games");

            migrationBuilder.DropTable(
                name: "GameAppUsersStats");

            migrationBuilder.DropIndex(
                name: "IX_Games_LobbyId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_LobbyId1",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "IsFinished",
                table: "RacingCars");

            migrationBuilder.DropColumn(
                name: "LobbyId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "LobbyId1",
                table: "Games");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "Games",
                newName: "GameState");

            migrationBuilder.RenameColumn(
                name: "ModeId",
                table: "Games",
                newName: "GameModeId");

            migrationBuilder.RenameIndex(
                name: "IX_Games_ModeId",
                table: "Games",
                newName: "IX_Games_GameModeId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Games_GameModes_GameModeId",
                table: "Games",
                column: "GameModeId",
                principalTable: "GameModes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
