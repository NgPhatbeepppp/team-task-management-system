using System.ComponentModel.DataAnnotations;

namespace TeamTaskManagementSystem.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }

        [MaxLength(255)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public int? AssignedTo { get; set; }
        public User User { get; set; }

        [MaxLength(50)]
        public string Status { get; set; }

        public DateTime Deadline { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}
