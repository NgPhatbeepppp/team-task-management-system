namespace TeamTaskManagementSystem.DTOs
{
    public class CurrentUserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string? FullName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? JobTitle { get; set; }
    }
}
