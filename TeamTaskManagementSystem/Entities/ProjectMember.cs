using System.Text.Json.Serialization;

namespace TeamTaskManagementSystem.Entities
{
    public class ProjectMember
    {
        public int ProjectId { get; set; }
        [JsonIgnore]
        public Project? Project { get; set; }

        public int UserId { get; set; }
        [JsonIgnore]
        public User? User { get; set; }

        public string RoleInProject { get; set; } // "ProjectLeader", "Contributor"
    }
}
