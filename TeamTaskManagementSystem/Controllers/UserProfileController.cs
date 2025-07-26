using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamTaskManagementSystem.DTOs;
using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Interfaces.IAuth_User;
using TeamTaskManagementSystem.ViewModels;

namespace TeamTaskManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserProfileController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet("me")]
        public async Task<ActionResult<CurrentUserDto>> GetMyProfile()
        {
            var userId = GetUserId();
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized(); // Nếu không tìm thấy user từ token, có lỗi xác thực
            }

            var profile = await _userRepository.GetUserProfileByUserIdAsync(userId);

            // Tạo DTO để trả về cho Frontend
            var currentUserDto = new CurrentUserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                FullName = profile?.FullName,
                AvatarUrl = profile?.AvatarUrl,
                JobTitle = profile?.JobTitle
            };

            return Ok(currentUserDto);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UserProfileUpdateRequest request)
        {
            var profile = await _userRepository.GetUserProfileByUserIdAsync(GetUserId());
            if (profile == null)
            {
                // Nếu chưa có profile, tạo mới
                profile = new Entities.UserProfile { UserId = GetUserId() };
                await _userRepository.AddUserProfileAsync(profile);
            }

            profile.FullName = request.FullName;
            profile.Bio = request.Bio;
            profile.AvatarUrl = request.AvatarUrl;
            profile.Gender = request.Gender;
            profile.JobTitle = request.JobTitle;
            profile.PhoneNumber = request.PhoneNumber;

            await _userRepository.SaveChangesAsync();
            return NoContent();
        }
    }
}
