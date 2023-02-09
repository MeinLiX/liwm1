using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    /// <inheritdoc />
    public partial class AddUserNameToAnonymousUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "AnonymousUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "AnonymousUsers");
        }
    }
}
