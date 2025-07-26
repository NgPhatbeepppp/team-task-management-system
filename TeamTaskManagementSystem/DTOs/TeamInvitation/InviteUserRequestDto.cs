using System.ComponentModel.DataAnnotations;

namespace TeamTaskManagementSystem.DTOs.TeamInvitation
{
    public class InviteUserRequestDto
    {
        [Required]
        public int TargetUserId { get; set; }
    }
}
