# 📌 Team Task Management System

Một hệ thống **back-end** toàn diện được xây dựng bằng **ASP.NET Core Web API** để quản lý công việc, dự án và nhóm.  
Hệ thống cung cấp bộ API mạnh mẽ để xử lý người dùng, xác thực, quản lý dự án, phân công công việc và cộng tác nhóm.

---

## ✨ Tính năng chính

### **1. Quản lý Xác thực & Người dùng**
- Đăng ký và đăng nhập an toàn với **JWT (JSON Web Tokens)**.
- Chức năng *Quên mật khẩu* và *Đặt lại mật khẩu*.
- Quản lý thông tin cá nhân (**User Profile**).

### **2. Quản lý Nhóm (Team)**
- Tạo, cập nhật, xóa nhóm.
- Mỗi nhóm có **KeyCode** duy nhất để mời vào dự án.
- Quản lý thành viên: thêm, xóa, phân quyền **Team Leader**.
- Hệ thống mời thành viên qua KeyCode hoặc username/email.

### **3. Quản lý Dự án (Project)**
- Tạo, cập nhật, xóa dự án.
- Thêm/xóa thành viên hoặc toàn bộ nhóm vào dự án.
- Người tạo dự án mặc định là **Project Leader**.
- Hệ thống mời linh hoạt: mời từng người hoặc cả nhóm.

### **4. Quản lý Công việc (Task)**
- Tạo, cập nhật, xóa công việc.
- Giao việc cho nhiều thành viên.
- Thuộc tính chi tiết: tiêu đề, mô tả, ngày bắt đầu, deadline, độ ưu tiên, trạng thái.
- Quản lý **Checklist** cho từng công việc.

### **5. Hệ thống Trạng thái Công việc (Status)**
- Quản lý các trạng thái (To Do, In Progress, Done) theo từng dự án.
- Tạo, sửa, xóa, sắp xếp lại các cột trạng thái.

---

## 🚀 Công nghệ sử dụng
- **Framework:** .NET 8.0  
- **API:** ASP.NET Core Web API  
- **Database:** Microsoft SQL Server  
- **ORM:** Entity Framework Core 8  
- **Authentication:** JWT Bearer Token  
- **API Documentation:** Swashbuckle (Swagger)  

---

## 🛠️ Cài đặt và Chạy dự án

### **Yêu cầu**
- .NET 8 SDK  
- Microsoft SQL Server  
- SSMS hoặc Azure Data Studio  

### **Các bước cài đặt**

1. **Clone repository**
   ```bash
   git clone https://github.com/NgPhatbeepppp/team-task-management-system.git
   cd team-task-management-system/TeamTaskManagementSystem
   ```

2. **Cấu hình Connection String**
   - Mở `appsettings.json`
   - Sửa giá trị:
     ```json
     "ConnectionStrings": {
       "DefaultConnection": "Server=TEN_SERVER_CUA_BAN;Database=TeamTaskDB;Trusted_Connection=True;TrustServerCertificate=True;"
     }
     ```

3. **Áp dụng Migrations**
   ```bash
   dotnet ef database update
   ```

4. **Chạy ứng dụng**
   ```bash
   dotnet run
   ```

5. **Truy cập API**
   - API: [http://localhost:5250](http://localhost:5250)  
   - Swagger UI: [http://localhost:5250/swagger](http://localhost:5250/swagger)  

---

## 📚 API Endpoints (Tiêu biểu)

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| POST   | `/api/Auth/register` | Đăng ký người dùng mới |
| POST   | `/api/Auth/login` | Đăng nhập & nhận JWT token |
| GET    | `/api/UserProfile/me` | Lấy thông tin người dùng hiện tại |
| POST   | `/api/Team` | Tạo nhóm mới |
| GET    | `/api/Team/mine` | Lấy danh sách nhóm của tôi |
| POST   | `/api/team/{teamId}/invitations` | Mời thành viên vào nhóm |
| POST   | `/api/Project` | Tạo dự án mới |
| GET    | `/api/Project/mine` | Lấy danh sách dự án của tôi |
| POST   | `/api/projects/{projectId}/invitations/user` | Mời người dùng vào dự án |
| POST   | `/api/projects/{projectId}/invitations/team` | Mời nhóm vào dự án |
| POST   | `/api/Tasks` | Tạo công việc mới |
| GET    | `/api/Tasks/project/{projectId}` | Lấy danh sách công việc của dự án |
| GET    | `/api/Tasks/mine` | Lấy công việc của tôi |
| PUT    | `/api/Tasks/{id}` | Cập nhật công việc |
| GET    | `/api/projects/{projectId}/statuses` | Lấy các trạng thái của dự án |
| POST   | `/api/projects/{projectId}/statuses` | Tạo trạng thái mới |

> 📌 **Lưu ý:** Xem đầy đủ endpoints trong `/swagger`.

---

## 📈 Sơ đồ Cơ sở dữ liệu (Database Schema)

```mermaid
erDiagram
    %% =========================
    %% Entities & Relationships
    %% =========================

    %% One-to-one: Users <> UserProfiles
    Users ||--|| UserProfiles : has

    %% Many-to-many qua bảng trung gian
    Users }o--o{ Teams : via TeamMembers
    Users }o--o{ Projects : via ProjectMembers
    Teams }o--o{ Projects : via ProjectTeams

    %% Projects -> Tasks, ProjectStatuses (1-n)
    Projects ||--o{ TaskItem : has
    Projects ||--o{ ProjectStatuses : has

    %% Tasks -> ChecklistItems (1-n)
    TaskItem ||--o{ ChecklistItems : has

    %% Tasks <-> Users (n-n) qua TaskAssignees
    TaskItem }o--o{ Users : via TaskAssignees

    %% Invitations
    Teams ||--o{ TeamInvitations : creates
    Users ||--o{ TeamInvitations : invited
    Users ||--o{ TeamInvitations : invitedBy

    Projects ||--o{ ProjectInvitations : creates
    Users ||--o{ ProjectInvitations : invited
    Teams ||--o{ ProjectInvitations : invitedTeam
    Users ||--o{ ProjectInvitations : invitedBy

    %% Comments
    TaskItem ||--o{ Comments : has
    Users ||--o{ Comments : writes

    %% ActivityLogs & Notifications
    Users ||--o{ ActivityLogs : performs
    Projects ||--o{ ActivityLogs : context
    Users ||--o{ Notifications : receives

    %% =========================
    %% Tables & Columns
    %% =========================

    Users {
        int Id PK
        nvarchar(50) Username "NOT NULL, UNIQUE"
        nvarchar(MAX) PasswordHash "NOT NULL"
        nvarchar(255) Email "NOT NULL, UNIQUE"
        nvarchar(MAX) Role "e.g. User/Admin"
        datetime2 CreatedAt
        bit IsActive
        nvarchar(MAX) PasswordResetToken
        datetime2 ResetTokenExpires
    }

    UserProfiles {
        int UserId PK, FK "-> Users.Id"
        nvarchar(100) FullName
        nvarchar(255) AvatarUrl
        nvarchar(500) Bio
        nvarchar(20) PhoneNumber
        nvarchar(100) JobTitle
        nvarchar(20) Gender
    }

    Teams {
        int Id PK
        nvarchar(20) KeyCode "NOT NULL, UNIQUE"
        nvarchar(100) Name "NOT NULL"
        nvarchar(500) Description
        datetime2 CreatedAt
        int CreatedByUserId FK "-> Users.Id"
    }

    Projects {
        int Id PK
        nvarchar(20) KeyCode "NOT NULL, UNIQUE"
        nvarchar(100) Name "NOT NULL"
        nvarchar(500) Description
        datetime2 CreatedAt
        int CreatedByUserId FK "-> Users.Id"
    }

    TaskItem {
        int Id PK
        nvarchar(255) Title "NOT NULL"
        nvarchar(MAX) Description
        datetime2 StartDate
        datetime2 Deadline
        nvarchar(20) Priority "Low/Medium/High"
        datetime2 CreatedAt
        int ProjectId FK "-> Projects.Id"
        int StatusId FK "-> ProjectStatuses.Id"
        int ParentTaskId FK "self -> TaskItem.Id"
        int CreatedByUserId FK "-> Users.Id"
    }

    ProjectStatuses {
        int Id PK
        nvarchar(50) Name "NOT NULL"
        nvarchar(7) Color "HEX e.g. #FFFFFF"
        int Order
        int ProjectId FK "-> Projects.Id"
    }

    ChecklistItems {
        int Id PK
        nvarchar(255) Content "NOT NULL"
        bit IsCompleted
        int Order
        int TaskId FK "-> TaskItem.Id"
    }

    TeamMembers {
        int TeamId PK, FK "-> Teams.Id"
        int UserId PK, FK "-> Users.Id"
        nvarchar(50) RoleInTeam "TeamLeader/Member"
    }

    ProjectMembers {
        int ProjectId PK, FK "-> Projects.Id"
        int UserId PK, FK "-> Users.Id"
        nvarchar(MAX) RoleInProject "ProjectLeader/Contributor"
    }

    ProjectTeams {
        int ProjectId PK, FK "-> Projects.Id"
        int TeamId PK, FK "-> Teams.Id"
    }

    TaskAssignees {
        int TaskId PK, FK "-> TaskItem.Id"
        int UserId PK, FK "-> Users.Id"
    }

    TeamInvitations {
        int Id PK
        int TeamId FK "-> Teams.Id"
        int InvitedUserId FK "-> Users.Id"
        int InvitedByUserId FK "-> Users.Id"
        nvarchar(20) Status "Pending/Accepted/Rejected"
        datetime2 CreatedAt
    }

    ProjectInvitations {
        int Id PK
        int ProjectId FK "-> Projects.Id"
        int InvitedUserId FK "-> Users.Id"
        int InvitedTeamId FK "-> Teams.Id"
        int InvitedByUserId FK "-> Users.Id"
        nvarchar(MAX) Status
        datetime2 CreatedAt
    }

    Comments {
        int Id PK
        nvarchar(1000) Content
        int TaskId FK "-> TaskItem.Id"
        int UserId FK "-> Users.Id"
    }

    ActivityLogs {
        int Id PK
        nvarchar(100) Action "e.g. CREATED_TASK"
        nvarchar(500) Description
        datetime2 CreatedAt
        int UserId FK "-> Users.Id"
        int ProjectId FK "-> Projects.Id"
    }

    Notifications {
        int Id PK
        nvarchar(50) Type
        nvarchar(500) Message
        bit IsRead
        int UserId FK "-> Users.Id"
    }
```
---

Tất cả quan hệ và ràng buộc được định nghĩa trong `AppDbContext.cs` để đảm bảo tính toàn vẹn dữ liệu.
