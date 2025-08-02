// TeamTaskManagementSystem/Entities/TaskAssignee.cs

using System.Text.Json.Serialization;

namespace TeamTaskManagementSystem.Entities
{
    public class TaskAssignee
    {
        public int TaskId { get; set; }
        [JsonIgnore]
        public virtual TaskItem Task { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}