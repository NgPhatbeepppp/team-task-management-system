using System.ComponentModel.DataAnnotations;

namespace TeamTaskManagementSystem.ViewModels
{
    public class AuthRegisterRequest
    {
        [Required]
        public string Username { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        [Required]
        [RegularExpression("^(Nam|Nữ|Khác)$", ErrorMessage = "Giới tính phải là 'Nam', 'Nữ' hoặc 'Khác'")]
        public string Gender { get; set; }

        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
    }
}