using System.ComponentModel.DataAnnotations;

namespace TeamTaskManagementSystem.Entities
{
    public class ActivityLog
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Action { get; set; } // Ví dụ: "CREATED_TASK"

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } // Ví dụ: "User 'A' đã tạo task 'B'"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Khóa ngoại (Foreign Keys)
        public int UserId { get; set; }
        public virtual User User { get; set; }

        public int? ProjectId { get; set; }
        public virtual Project? Project { get; set; }
    }
}
