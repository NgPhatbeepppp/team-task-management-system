namespace TeamTaskManagementSystem.DTOs
{
    public class InvitationDto
    {
        public int InvitationId { get; set; }
        public string InvitationType { get; set; } // "Project" hoặc "Team"
        public string TargetName { get; set; } // Tên của Project hoặc Team
        public int TargetId { get; set; } // Id của Project hoặc Team
        public string InviterName { get; set; } // Tên người mời
        public DateTime SentAt { get; set; }
    }
}