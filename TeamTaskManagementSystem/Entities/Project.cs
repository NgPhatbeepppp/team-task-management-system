using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TeamTaskManagementSystem.Entities
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Khóa ngoại (Foreign Key)
        public int CreatedByUserId { get; set; }
        [JsonIgnore]
        public virtual User? CreatedByUser { get; set; }

       
        // --- BỔ SUNG CÁC THUỘC TÍNH ĐIỀU HƯỚNG ---
        public virtual ICollection<ProjectTeam> Teams { get; set; } = new List<ProjectTeam>();
        public virtual ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();
        public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
        public virtual ICollection<ProjectStatus> Statuses { get; set; } = new List<ProjectStatus>();
    }
}
