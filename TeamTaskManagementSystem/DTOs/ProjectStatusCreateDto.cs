using System.ComponentModel.DataAnnotations;

namespace TeamTaskManagementSystem.DTOs
{
    public class ProjectStatusCreateDto
    {
        [Required(ErrorMessage = "Tên trạng thái không được để trống.")]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(7, ErrorMessage = "Mã màu phải ở định dạng Hex (ví dụ: #FFFFFF).")]
        public string? Color { get; set; } // FE có thể gửi hoặc không, BE sẽ gán màu mặc định

        [Required]
        public int ProjectId { get; set; }
    }
}