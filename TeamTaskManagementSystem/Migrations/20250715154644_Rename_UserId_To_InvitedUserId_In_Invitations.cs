using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamTaskManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class Rename_UserId_To_InvitedUserId_In_Invitations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectInvitations_Teams_TeamId",
                table: "ProjectInvitations");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectInvitations_Users_UserId",
                table: "ProjectInvitations");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ProjectInvitations",
                newName: "InvitedUserId");

            migrationBuilder.RenameColumn(
                name: "TeamId",
                table: "ProjectInvitations",
                newName: "InvitedTeamId");

            migrationBuilder.RenameColumn(
                name: "SentAt",
                table: "ProjectInvitations",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectInvitations_UserId",
                table: "ProjectInvitations",
                newName: "IX_ProjectInvitations_InvitedUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectInvitations_TeamId",
                table: "ProjectInvitations",
                newName: "IX_ProjectInvitations_InvitedTeamId");

            migrationBuilder.AddColumn<int>(
                name: "InvitedByUserId",
                table: "ProjectInvitations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectInvitations_InvitedByUserId",
                table: "ProjectInvitations",
                column: "InvitedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectInvitations_Teams_InvitedTeamId",
                table: "ProjectInvitations",
                column: "InvitedTeamId",
                principalTable: "Teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectInvitations_Users_InvitedByUserId",
                table: "ProjectInvitations",
                column: "InvitedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectInvitations_Users_InvitedUserId",
                table: "ProjectInvitations",
                column: "InvitedUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectInvitations_Teams_InvitedTeamId",
                table: "ProjectInvitations");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectInvitations_Users_InvitedByUserId",
                table: "ProjectInvitations");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectInvitations_Users_InvitedUserId",
                table: "ProjectInvitations");

            migrationBuilder.DropIndex(
                name: "IX_ProjectInvitations_InvitedByUserId",
                table: "ProjectInvitations");

            migrationBuilder.DropColumn(
                name: "InvitedByUserId",
                table: "ProjectInvitations");

            migrationBuilder.RenameColumn(
                name: "InvitedUserId",
                table: "ProjectInvitations",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "InvitedTeamId",
                table: "ProjectInvitations",
                newName: "TeamId");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "ProjectInvitations",
                newName: "SentAt");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectInvitations_InvitedUserId",
                table: "ProjectInvitations",
                newName: "IX_ProjectInvitations_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectInvitations_InvitedTeamId",
                table: "ProjectInvitations",
                newName: "IX_ProjectInvitations_TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectInvitations_Teams_TeamId",
                table: "ProjectInvitations",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectInvitations_Users_UserId",
                table: "ProjectInvitations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
