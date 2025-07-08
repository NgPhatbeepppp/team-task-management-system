using System.Text.Json.Serialization;

namespace TeamTaskManagementSystem.Entities
{
    public class ProjectTeam
    {
        public int ProjectId { get; set; }
        [JsonIgnore]
        public Project Project { get; set; }

        public int TeamId { get; set; }
        [JsonIgnore]
        public Team Team { get; set; }
    }
}
