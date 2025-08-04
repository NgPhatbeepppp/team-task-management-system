using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamTaskManagementSystem.DTOs
{
    public class ProjectStatusReorderDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "Phải có ít nhất một ID trạng thái.")]
        public List<int> StatusIdsInOrder { get; set; }
    }
}