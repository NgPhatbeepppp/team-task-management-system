using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Interfaces;
using TeamTaskManagementSystem.ViewModels;

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

        public async Task<string?> RegisterAsync(AuthRegisterRequest request)
        {
            if (await _userRepository.IsUsernameOrEmailTakenAsync(request.Username, request.Email))
                return null;

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

            return GenerateJwtToken(user);
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
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

