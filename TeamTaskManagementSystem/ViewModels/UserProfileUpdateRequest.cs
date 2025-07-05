namespace TeamTaskManagementSystem.ViewModels
{
    public class UserProfileUpdateRequest
    {
        public string? FullName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Bio { get; set; }
        public string? JobTitle { get; set; }
        public string Gender { get; set; } = "Khác"; // default nếu cần
        public string? PhoneNumber { get; set; }
    }
}
