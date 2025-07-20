using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace TeamTaskManagementSystem.Entities
{
    public class Team
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key
        public int CreatedByUserId { get; set; }
        [JsonIgnore]        
        public virtual User? CreatedByUser { get; set; }

      
        // Một nhóm có nhiều thành viên
        
        [JsonPropertyName("teamMembers")]
        public virtual ICollection<TeamMember> Members { get; set; } = new List<TeamMember>();

        // Một nhóm tham gia nhiều dự án thông qua bảng nối ProjectTeam
        [JsonIgnore]
        public virtual ICollection<ProjectTeam> Projects { get; set; } = new List<ProjectTeam>();
    }
}
