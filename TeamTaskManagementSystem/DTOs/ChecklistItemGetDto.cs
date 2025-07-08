using System.ComponentModel.DataAnnotations;

namespace TeamTaskManagementSystem.DTOs
{
    public class ChecklistItemGetDto
    {
        public int? Id { get; set; } // Dùng cho update

        [Required]
        [MaxLength(255)]
        public string Content { get; set; }
        public int Order { get; set; } = 0; // mặc định hoặc do FE gửi

        public bool IsCompleted { get; set; } = false;

        [Required]
        public int TaskId { get; set; }
    }
}
