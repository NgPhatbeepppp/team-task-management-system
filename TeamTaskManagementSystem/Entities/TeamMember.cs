using System.ComponentModel.DataAnnotations;

namespace TeamTaskManagementSystem.Entities
{
    public class TeamMember
    {
        public int TeamId { get; set; }
        public Team Team { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        [MaxLength(50)]
        public string RoleInTeam { get; set; }
    }
}
