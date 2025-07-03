using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamTaskManagementSystem.Entities
{
    public class UserProfile
    {
        [Key]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [MaxLength(100)]
        public string? FullName { get; set; }

        [MaxLength(255)]
        public string? AvatarUrl { get; set; }

        [MaxLength(500)]
        public string? Bio { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(100)]
        public string? JobTitle { get; set; }

        [MaxLength(20)]
        public string? Gender { get; set; }

        // Mối quan hệ một-một (One-to-One relationship)
        public virtual User User { get; set; }
    }
}
