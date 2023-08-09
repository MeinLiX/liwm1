using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    /// <inheritdoc />
    public partial class AddLevels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameModeAppUserStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Points = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalPointsForNewLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    GameModeId = table.Column<int>(type: "INTEGER", nullable: false),
                    AppUserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameModeAppUserStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameModeAppUserStats_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameModeAppUserStats_GameModes_GameModeId",
                        column: x => x.GameModeId,
                        principalTable: "GameModes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameModeAppUserStats_AppUserId",
                table: "GameModeAppUserStats",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GameModeAppUserStats_GameModeId",
                table: "GameModeAppUserStats",
                column: "GameModeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameModeAppUserStats");
        }
    }
}
