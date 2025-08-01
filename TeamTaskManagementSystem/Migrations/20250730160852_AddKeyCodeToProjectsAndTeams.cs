using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamTaskManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddKeyCodeToProjectsAndTeams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KeyCode",
                table: "Teams",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "KeyCode",
                table: "Projects",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KeyCode",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "KeyCode",
                table: "Projects");
        }
    }
}
