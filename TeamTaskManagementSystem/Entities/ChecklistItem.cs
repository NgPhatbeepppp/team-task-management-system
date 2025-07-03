using System.ComponentModel.DataAnnotations;

namespace TeamTaskManagementSystem.Entities
{
    public class ChecklistItem
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Content { get; set; }

        public bool IsCompleted { get; set; } = false;

        public int Order { get; set; }

        // Khóa ngoại (Foreign Key)
        public int TaskId { get; set; }
        public virtual TaskItem Task { get; set; }
    }
}
