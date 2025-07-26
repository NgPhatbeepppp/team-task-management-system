using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamTaskManagementSystem.Migrations
{
    
    public partial class Fix_TeamDeletion_Cascade : Migration
    {
       
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectInvitations_Teams_InvitedTeamId",
                table: "ProjectInvitations");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectInvitations_Teams_InvitedTeamId",
                table: "ProjectInvitations",
                column: "InvitedTeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectInvitations_Teams_InvitedTeamId",
                table: "ProjectInvitations");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectInvitations_Teams_InvitedTeamId",
                table: "ProjectInvitations",
                column: "InvitedTeamId",
                principalTable: "Teams",
                principalColumn: "Id");
        }
    }
}
