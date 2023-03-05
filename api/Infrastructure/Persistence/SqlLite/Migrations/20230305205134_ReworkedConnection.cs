using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    /// <inheritdoc />
    public partial class ReworkedConnection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Connection_Lobbies_LobbyId",
                table: "Connection");

            migrationBuilder.DropForeignKey(
                name: "FK_Connection_Lobbies_LobbyId1",
                table: "Connection");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Connection",
                table: "Connection");

            migrationBuilder.DropIndex(
                name: "IX_Connection_LobbyId1",
                table: "Connection");

            migrationBuilder.DropColumn(
                name: "LobbyId1",
                table: "Connection");

            migrationBuilder.RenameTable(
                name: "Connection",
                newName: "Connections");

            migrationBuilder.RenameIndex(
                name: "IX_Connection_LobbyId",
                table: "Connections",
                newName: "IX_Connections_LobbyId");

            migrationBuilder.AlterColumn<int>(
                name: "LobbyId",
                table: "Connections",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ConnectionState",
                table: "Connections",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Connections",
                table: "Connections",
                column: "ConnectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Connections_Lobbies_LobbyId",
                table: "Connections",
                column: "LobbyId",
                principalTable: "Lobbies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Connections_Lobbies_LobbyId",
                table: "Connections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Connections",
                table: "Connections");

            migrationBuilder.DropColumn(
                name: "ConnectionState",
                table: "Connections");

            migrationBuilder.RenameTable(
                name: "Connections",
                newName: "Connection");

            migrationBuilder.RenameIndex(
                name: "IX_Connections_LobbyId",
                table: "Connection",
                newName: "IX_Connection_LobbyId");

            migrationBuilder.AlterColumn<int>(
                name: "LobbyId",
                table: "Connection",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "LobbyId1",
                table: "Connection",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Connection",
                table: "Connection",
                column: "ConnectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Connection_LobbyId1",
                table: "Connection",
                column: "LobbyId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Connection_Lobbies_LobbyId",
                table: "Connection",
                column: "LobbyId",
                principalTable: "Lobbies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Connection_Lobbies_LobbyId1",
                table: "Connection",
                column: "LobbyId1",
                principalTable: "Lobbies",
                principalColumn: "Id");
        }
    }
}
