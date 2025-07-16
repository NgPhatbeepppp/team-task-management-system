using System;

namespace TeamTaskManagementSystem.Entities
{
    public class ProjectInvitation
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public virtual Project? Project { get; set; }

        public int? InvitedUserId { get; set; }
        public User? InvitedUser { get; set; }

        public int? InvitedTeamId { get; set; }
        public Team? InvitedTeam { get; set; }

        public int InvitedByUserId { get; set; }
        public virtual User? InvitedByUser { get; set; }

        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
