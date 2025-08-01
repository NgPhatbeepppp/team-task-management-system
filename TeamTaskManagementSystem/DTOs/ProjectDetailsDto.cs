// TeamTaskManagementSystem/DTOs/ProjectDetailsDto.cs
using System;
using System.Collections.Generic;

namespace TeamTaskManagementSystem.DTOs
{
    public class ProjectDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedByUserId { get; set; }
        public ICollection<ProjectMemberDto> Members { get; set; } = new List<ProjectMemberDto>();
        public ICollection<TeamDto> Teams { get; set; } = new List<TeamDto>();
    }
}