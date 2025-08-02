using System.ComponentModel.DataAnnotations;

namespace TeamTaskManagementSystem.DTOs
{
    public class UpdateTaskStatusDto
    {
        [Required]
        public int NewStatusId { get; set; }
    }
}