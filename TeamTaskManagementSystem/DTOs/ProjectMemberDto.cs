// TeamTaskManagementSystem/DTOs/ProjectMemberDto.cs
namespace TeamTaskManagementSystem.DTOs
{
    public class ProjectMemberDto
    {
        public UserDto User { get; set; }
        public string RoleInProject { get; set; }
    }
}