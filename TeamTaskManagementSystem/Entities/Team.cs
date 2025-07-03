using System.ComponentModel.DataAnnotations;

namespace TeamTaskManagementSystem.Entities
{
    public class Team
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key
        public int CreatedByUserId { get; set; }
        public virtual User CreatedByUser { get; set; }

        // --- SỬA LỖI Ở ĐÂY ---
        // Một nhóm có nhiều thành viên
        public virtual ICollection<TeamMember> Members { get; set; } = new List<TeamMember>();

        // Một nhóm tham gia nhiều dự án thông qua bảng nối ProjectTeam
        public virtual ICollection<ProjectTeam> Projects { get; set; } = new List<ProjectTeam>();
    }
}
