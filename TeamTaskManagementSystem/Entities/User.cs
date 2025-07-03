using System.ComponentModel.DataAnnotations;

namespace TeamTaskManagementSystem.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [MaxLength(255)]
        public string Email { get; set; }

        public string Role { get; set; } = "User";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        // --- BỔ SUNG CÁC THUỘC TÍNH ĐIỀU HƯỚNG ---
        public virtual UserProfile UserProfile { get; set; } // Quan hệ 1-1 (One-to-One relationship)
        public virtual ICollection<TeamMember> Teams { get; set; } = new List<TeamMember>();
        public virtual ICollection<ProjectMember> Projects { get; set; } = new List<ProjectMember>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
