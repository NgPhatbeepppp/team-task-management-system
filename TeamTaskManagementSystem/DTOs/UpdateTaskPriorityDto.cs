using System.ComponentModel.DataAnnotations;

namespace TeamTaskManagementSystem.DTOs
{
    public class UpdateTaskPriorityDto
    {
        [Required(ErrorMessage = "Độ ưu tiên không được để trống.")]
        [RegularExpression("^(Low|Medium|High)$", ErrorMessage = "Độ ưu tiên phải là 'Low', 'Medium' hoặc 'High'.")]
        public string Priority { get; set; }
    }
}