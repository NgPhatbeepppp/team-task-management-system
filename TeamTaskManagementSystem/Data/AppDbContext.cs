using Microsoft.EntityFrameworkCore;
using TeamTaskManagementSystem.Entities;

namespace TeamTaskManagementSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        #region DbSets
        // --- CORE TABLES ---
        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Project> Projects { get; set; }

        // --- TASK MANAGEMENT TABLES ---
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<ProjectStatus> ProjectStatuses { get; set; }
        public DbSet<ChecklistItem> ChecklistItems { get; set; }

        // --- RELATIONSHIP & COLLABORATION TABLES ---
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }
        public DbSet<ProjectTeam> ProjectTeams { get; set; }
        public DbSet<ProjectInvitation> ProjectInvitations { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<TeamInvitation> TeamInvitations { get; set; }
        public DbSet<TaskAssignee> TaskAssignees { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Composite Keys for Many-to-Many relationships
            modelBuilder.Entity<TeamMember>().HasKey(tm => new { tm.TeamId, tm.UserId });
            modelBuilder.Entity<ProjectMember>().HasKey(pm => new { pm.ProjectId, pm.UserId });
            modelBuilder.Entity<ProjectTeam>().HasKey(pt => new { pt.ProjectId, pt.TeamId });
            modelBuilder.Entity<TaskAssignee>().HasKey(ta => new { ta.TaskId, ta.UserId });
            #endregion

            #region Relationship Configurations
            // User <-> UserProfile (One-to-One)
            modelBuilder.Entity<User>()
                .HasOne(u => u.UserProfile)
                .WithOne(p => p.User)
                .HasForeignKey<UserProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa User thì xóa luôn Profile

            // Team <-> User (Many-to-Many)
            modelBuilder.Entity<TeamMember>()
                .HasOne(tm => tm.Team).WithMany(t => t.Members).HasForeignKey(tm => tm.TeamId);
            modelBuilder.Entity<TeamMember>()
                .HasOne(tm => tm.User).WithMany(u => u.Teams).HasForeignKey(tm => tm.UserId);

            // Project <-> User (Many-to-Many)
            modelBuilder.Entity<ProjectMember>()
                .HasOne(pm => pm.Project).WithMany(p => p.Members).HasForeignKey(pm => pm.ProjectId);
            modelBuilder.Entity<ProjectMember>()
                .HasOne(pm => pm.User).WithMany(u => u.Projects).HasForeignKey(pm => pm.UserId);

            // Project <-> Team (Many-to-Many)
            modelBuilder.Entity<ProjectTeam>()
                .HasOne(pt => pt.Project).WithMany(p => p.Teams).HasForeignKey(pt => pt.ProjectId);
            modelBuilder.Entity<ProjectTeam>()
                .HasOne(pt => pt.Team).WithMany(t => t.Projects).HasForeignKey(pt => pt.TeamId);
            #endregion

            #region ON DELETE Behavior Rules (QUAN TRỌNG)
            // === QUY TẮC PHÁ VỠ CHUỖI DOMINO (BREAKING THE CYCLE) ===

            // 1. Cấm xóa User nếu họ vẫn là người tạo ra các thực thể quan trọng khác.
            modelBuilder.Entity<Project>()
                .HasOne(p => p.CreatedByUser).WithMany().HasForeignKey(p => p.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Team>()
                .HasOne(t => t.CreatedByUser).WithMany().HasForeignKey(t => t.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.CreatedByUser).WithMany().HasForeignKey(t => t.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);

            // 2. Cấm xóa Project nếu nó vẫn còn chứa các Statuses.
            //    Điều này phá vỡ vòng lặp: Project -> Statuses -> Tasks <- Project
            modelBuilder.Entity<ProjectStatus>()
                .HasOne(ps => ps.Project)
                .WithMany(p => p.Statuses)
                .HasForeignKey(ps => ps.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            // === CÁC QUY TẮC XÓA AN TOÀN KHÁC ===

           
            // 4. Nếu xóa Status, các Task đang dùng nó sẽ trở thành "không có status".
            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.Status)
                .WithMany(ps => ps.Tasks)
                .HasForeignKey(t => t.StatusId)
                .OnDelete(DeleteBehavior.SetNull);

            // 5. Cấm xóa Task cha nếu nó vẫn còn Task con.
            modelBuilder.Entity<TaskItem>()
                .HasMany(t => t.Subtasks)
                .WithOne(t => t.ParentTask)
                .HasForeignKey(t => t.ParentTaskId)
                .OnDelete(DeleteBehavior.Restrict);

            // 6. Xóa Task hoặc User sẽ xóa các Comment liên quan.
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Task).WithMany(t => t.Comments).HasForeignKey(c => c.TaskId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User).WithMany(u => u.Comments).HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade);
            // 7.Khi một Team bị xóa, tự động xóa các ProjectInvitation liên quan đến Team đó.
            modelBuilder.Entity<ProjectInvitation>()
               .HasOne(pi => pi.InvitedTeam)
               .WithMany()
               .HasForeignKey(pi => pi.InvitedTeamId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TeamInvitation>()
                .HasOne(ti => ti.InvitedUser)
                .WithMany()
                .HasForeignKey(ti => ti.InvitedUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TeamInvitation>()
                .HasOne(ti => ti.InvitedByUser)
                .WithMany()
                .HasForeignKey(ti => ti.InvitedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion
        }
    }
}
