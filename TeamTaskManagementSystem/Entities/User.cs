using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace TeamTaskManagementSystem.Entities
{
    public class User
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string Username { get; set; }
        [MaxLength(255)]
        public string PasswordHash { get; set; }
        [MaxLength(255)]
        public string Email { get; set; }
        [MaxLength(20)]
        public string Role { get; set; }

        public ICollection<Comment> Comments { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }
}
