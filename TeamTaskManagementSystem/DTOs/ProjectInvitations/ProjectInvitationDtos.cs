namespace TeamTaskManagementSystem.Dtos.ProjectInvitations
{
    public class InviteUserDto
    {
        public string Identifier { get; set; } = string.Empty;
    }

    public class InviteTeamDto
    {
        public int InvitedTeamId { get; set; }
    }
}
