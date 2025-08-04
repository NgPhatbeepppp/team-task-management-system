// TeamTaskManagementSystem/DTOs/TaskCreateDto.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TeamTaskManagementSystem.Helpers;

namespace TeamTaskManagementSystem.DTOs
{
    public class TaskCreateDto
    {
        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        public string? Description { get; set; }

        [JsonConverter(typeof(NullableDateTimeConverter))]
        public DateTime? StartDate { get; set; } // <--- THÊM DÒNG NÀY

        [JsonConverter(typeof(NullableDateTimeConverter))]
        public DateTime? Deadline { get; set; }
        public string Priority { get; set; } = "Medium";
        public int? StatusId { get; set; }

        [Required]
        public int ProjectId { get; set; }

        public List<int> AssignedUserIds { get; set; } = new List<int>();
    }
}