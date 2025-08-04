namespace TeamTaskManagementSystem.DTOs
{
    // DTO chứa thông tin tóm tắt của dự án
    public class ProjectSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string KeyCode { get; set; }
    }

    // DTO chính cho trang "Nhiệm vụ của tôi"
    public class TaskForUserDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? Deadline { get; set; }
        public string Priority { get; set; }
        public int? StatusId { get; set; }

        // Lồng thông tin tóm tắt của dự án
        public ProjectSummaryDto Project { get; set; }
    }
}