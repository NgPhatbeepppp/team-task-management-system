using System.ComponentModel.DataAnnotations;

namespace TeamTaskManagementSystem.DTOs
{
    public class ProjectStatusUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên trạng thái không được để trống.")]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(7, ErrorMessage = "Mã màu phải ở định dạng Hex (ví dụ: #FFFFFF).")]
        public string Color { get; set; }
    }
}