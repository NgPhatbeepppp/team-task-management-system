using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TeamTaskManagementSystem.Entities
{
    public class ChecklistItem
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Content { get; set; }

        public bool IsCompleted { get; set; } = false;

        public int Order { get; set; } // thêm vào

        // FK đến Task

        public int TaskId { get; set; }

        [JsonIgnore]
         public virtual TaskItem? Task { get; set; }
    }
}
