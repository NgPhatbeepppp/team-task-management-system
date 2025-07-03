using System.ComponentModel.DataAnnotations;

namespace TeamTaskManagementSystem.Entities
{
    public class ProjectStatus
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(7)]
        public string Color { get; set; } = "#808080"; // Màu xám mặc định (Default gray color)

        public int Order { get; set; }

        // Khóa ngoại (Foreign Key)
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }

        // Thuộc tính điều hướng (Navigation Property)
        public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
