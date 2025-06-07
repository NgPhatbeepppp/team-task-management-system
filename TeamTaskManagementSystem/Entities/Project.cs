using System.ComponentModel.DataAnnotations;

namespace TeamTaskManagementSystem.Entities
{
    public class Project
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public int TeamId { get; set; }
        public Team Team { get; set; }

        public ICollection<TaskItem> Tasks { get; set; }
    }
}
