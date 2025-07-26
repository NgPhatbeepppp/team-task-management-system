namespace TeamTaskManagementSystem.DTOs.TeamInvitation
{
    public class UserSearchResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        // Trạng thái của user đối với team: "Member", "Pending", "NotInvited"
        public string StatusInTeam { get; set; }
    }
}
