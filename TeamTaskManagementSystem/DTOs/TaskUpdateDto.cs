using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamTaskManagementSystem.DTOs
{
    public class TaskUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        public string? Description { get; set; }
        public DateTime? Deadline { get; set; }
        public string Priority { get; set; } = "Medium";
        public int? StatusId { get; set; }

        // Danh sách ID người dùng mới để cập nhật
        public List<int> AssignedUserIds { get; set; } = new List<int>();
    }
}