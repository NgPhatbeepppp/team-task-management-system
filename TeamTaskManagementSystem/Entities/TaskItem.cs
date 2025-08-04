using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TeamTaskManagementSystem.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        public string? Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? Deadline { get; set; }

        [Required]
        [MaxLength(20)]
        public string Priority { get; set; } = "Medium";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // --- Khóa ngoại ---
        [Required]
        public int ProjectId { get; set; }

       
        public int? StatusId { get; set; }
        public int? ParentTaskId { get; set; }
        public int CreatedByUserId { get; set; }

        

        // --- Thuộc tính điều hướng ---
        [JsonIgnore]
        public virtual Project? Project { get; set; }
       
        [JsonIgnore]
        public virtual ProjectStatus? Status { get; set; }
        [JsonIgnore]
        public virtual User? CreatedByUser { get; set; }
        [JsonIgnore]
        public virtual TaskItem? ParentTask { get; set; }
        [JsonPropertyName("taskAssignees")]
        public virtual ICollection<TaskAssignee> Assignees { get; set; } = new List<TaskAssignee>();
        [JsonIgnore]
        public virtual ICollection<TaskItem> Subtasks { get; set; } = new List<TaskItem>();
        [JsonIgnore]
        public virtual ICollection<ChecklistItem> ChecklistItems { get; set; } = new List<ChecklistItem>();
        [JsonIgnore]
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
