using System.ComponentModel.DataAnnotations;

namespace TeamTaskManagementSystem.ViewModels
{
    public class AuthLoginRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
