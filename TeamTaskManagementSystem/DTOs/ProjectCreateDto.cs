// TeamTaskManagementSystem/DTOs/ProjectCreateDto.cs
using System.ComponentModel.DataAnnotations;

namespace TeamTaskManagementSystem.DTOs
{
    public class ProjectCreateDto
    {
        [Required(ErrorMessage = "Tên dự án không được để trống.")]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
    }
}