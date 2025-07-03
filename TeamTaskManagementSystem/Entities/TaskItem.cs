using System.ComponentModel.DataAnnotations;

namespace TeamTaskManagementSystem.Entities
{
    public class TaskItem // Giữ tên là TaskItem như file của bạn
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        public string? Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? Deadline { get; set; }

        [MaxLength(20)]
        public string Priority { get; set; } = "Medium";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // --- CẬP NHẬT CÁC KHÓA NGOẠI ---
        public int ProjectId { get; set; }
        public int? AssignedToUserId { get; set; }
        public int? StatusId { get; set; }
        public int? ParentTaskId { get; set; }
        public int CreatedByUserId { get; set; }

        // --- BỔ SUNG CÁC THUỘC TÍNH ĐIỀU HƯỚNG ---
        public virtual Project Project { get; set; }
        public virtual User? AssignedTo { get; set; }
        public virtual ProjectStatus? Status { get; set; }
        public virtual User CreatedByUser { get; set; }

        public virtual TaskItem? ParentTask { get; set; }
        public virtual ICollection<TaskItem> Subtasks { get; set; } = new List<TaskItem>();
        public virtual ICollection<ChecklistItem> ChecklistItems { get; set; } = new List<ChecklistItem>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
