using System;
using System.ComponentModel.DataAnnotations;

namespace TeamTaskManagementSystem.Entities
{
    public class TeamInvitation
    {
        public int Id { get; set; }

        [Required]
        public int TeamId { get; set; }
        public virtual Team? Team { get; set; }

        [Required]
        public int InvitedUserId { get; set; }
        public virtual User? InvitedUser { get; set; }

        [Required]
        public int InvitedByUserId { get; set; }
        public virtual User? InvitedByUser { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Accepted, Rejected

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}