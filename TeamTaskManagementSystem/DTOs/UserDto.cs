// TeamTaskManagementSystem/DTOs/UserDto.cs
namespace TeamTaskManagementSystem.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string? FullName { get; set; } // Lấy từ UserProfile
    }
}