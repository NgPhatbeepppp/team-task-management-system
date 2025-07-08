using TeamTaskManagementSystem.Data;
using TeamTaskManagementSystem.Interfaces;
using TeamTaskManagementSystem.Repositories;
using TeamTaskManagementSystem.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

// ===================================================================
// (Đăng ký tất cả services)
// ===================================================================
var builder = WebApplication.CreateBuilder(args);

// 1. Thêm dịch vụ CORS
builder.Services.AddCors();

// 2. Thêm Database Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Thêm JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// 4. Thêm Repositories và Services (Dependency Injection)
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IChecklistItemService, ChecklistItemService>();
builder.Services.AddScoped<IChecklistItemRepository, ChecklistItemRepository>();

// 5. Thêm các services cần thiết cho Controller và Swagger
builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TeamTask API", Version = "v1" });

    // ⚠️ Thêm đoạn này để hỗ trợ Bearer Token
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập JWT token theo định dạng: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});



// ===================================================================
// XÂY DỰNG (Tạo ứng dụng)
// ===================================================================
var app = builder.Build();


// ===================================================================
// (Middleware Pipeline)
// ===================================================================

// Cấu hình đặc biệt cho môi trường DEVELOPMENT
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Chính sách CORS "dễ dãi" cho môi trường dev
    app.UseCors(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
}
else
{
    // Cấu hình cho môi trường PRODUCTION
    // app.UseCors(policy => policy.WithOrigins("https://your-frontend.com"));
    app.UseHsts();
}

// Chuyển hướng sang HTTPS
app.UseHttpsRedirection();

// Kích hoạt các lớp phòng thủ theo đúng thứ tự

app.UseAuthentication(); // Lính gác An Ninh
app.UseAuthorization();  // Lính gác Nội Vụ

// Ánh xạ các request đến đúng Controller
app.MapControllers();

// Chạy ứng dụng
app.Run();
