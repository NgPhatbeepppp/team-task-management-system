using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Interfaces.IAuth_User;
using TeamTaskManagementSystem.ViewModels;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace TeamTaskManagementSystem.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;
        private readonly IPasswordHasher<User> _hasher;

        public AuthService(IUserRepository userRepository, IConfiguration config)
        {
            _userRepository = userRepository;
            _config = config;
            _hasher = new PasswordHasher<User>();
        }
        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                // Trả về true để không tiết lộ email nào tồn tại trong hệ thống
                return true;
            }

            // Tạo token ngẫu nhiên
            var resetToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

            user.PasswordResetToken = resetToken;
            user.ResetTokenExpires = DateTime.UtcNow.AddHours(1); // Token hết hạn sau 1 giờ

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            // =================================================================
            // TODO: Implement email sending logic here
            // Gửi email cho người dùng với `resetToken`.
            // Ví dụ: var resetLink = $"https://your-frontend.com/reset-password?token={resetToken}";
            //        await _emailService.SendPasswordResetEmail(user.Email, resetLink);
            // =================================================================

            return true;
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            
            var user = await _userRepository.GetUserByPasswordResetTokenAsync(token);

            if (user == null || user.ResetTokenExpires < DateTime.UtcNow)
            {
                return false; // Token không hợp lệ hoặc đã hết hạn
            }

            // Cập nhật mật khẩu mới
            user.PasswordHash = _hasher.HashPassword(user, newPassword);

            // Xóa token sau khi sử dụng
            user.PasswordResetToken = null;
            user.ResetTokenExpires = null;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return true;
        }
        public async Task<RegisterResult> RegisterAsync(AuthRegisterRequest request)
        {
            if (await _userRepository.IsUsernameTakenAsync(request.Username))
                return RegisterResult.Fail("Tên đăng nhập đã được sử dụng.");

            if (await _userRepository.IsEmailTakenAsync(request.Email))
                return RegisterResult.Fail("Email đã được sử dụng.");

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                Role = "Member",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            user.PasswordHash = _hasher.HashPassword(user, request.Password);

            await _userRepository.AddUserAsync(user);
            await _userRepository.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(request.FullName) ||
                !string.IsNullOrWhiteSpace(request.PhoneNumber) ||
                !string.IsNullOrWhiteSpace(request.Gender))
            {
                var profile = new UserProfile
                {
                    UserId = user.Id,
                    FullName = request.FullName,
                    PhoneNumber = request.PhoneNumber,
                    Gender = request.Gender
                };
                await _userRepository.AddUserProfileAsync(profile);
                await _userRepository.SaveChangesAsync();
            }

            return RegisterResult.Ok(GenerateJwtToken(user));
        }


        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, user.Role),
        new Claim(ClaimTypes.Email, user.Email)
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],             
                audience: _config["Jwt:Audience"],         
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}

