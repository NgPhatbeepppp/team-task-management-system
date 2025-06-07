using System.ComponentModel.DataAnnotations;

namespace TeamTaskManagementSystem.Entities
{
    public class Notification
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        [MaxLength(50)]
        public string Type { get; set; }

        [MaxLength(500)]
        public string Message { get; set; }

        public bool IsRead { get; set; }
    }
}
