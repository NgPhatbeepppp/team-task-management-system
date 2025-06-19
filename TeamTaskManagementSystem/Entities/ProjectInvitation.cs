using System;

namespace TeamTaskManagementSystem.Entities
{
    public class ProjectInvitation
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public int? TeamId { get; set; }
        public Team? Team { get; set; }

        public int? UserId { get; set; }
        public User? User { get; set; }

        public string Status { get; set; } // "Pending", "Accepted", "Rejected"
        public DateTime SentAt { get; set; }
    }
}
