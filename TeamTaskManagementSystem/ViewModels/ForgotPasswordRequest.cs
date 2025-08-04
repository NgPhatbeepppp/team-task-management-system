using System.ComponentModel.DataAnnotations;

namespace TeamTaskManagementSystem.ViewModels
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}