using System.ComponentModel.DataAnnotations;

namespace TeamTaskManagementSystem.Entities
{
    public class Comment
    {
        public int Id { get; set; }

        public int TaskId { get; set; }
        public virtual TaskItem? Task { get; set; }

        public int UserId { get; set; }
        public virtual User? User { get; set; }

        [MaxLength(1000)]
        public string Content { get; set; }
    }
}
