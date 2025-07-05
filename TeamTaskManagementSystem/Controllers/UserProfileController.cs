using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Interfaces;
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
        public async Task<ActionResult<UserProfile?>> GetMyProfile()
        {
            var profile = await _userRepository.GetUserProfileByUserIdAsync(GetUserId());
            return Ok(profile);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UserProfileUpdateRequest request)
        {
            var profile = await _userRepository.GetUserProfileByUserIdAsync(GetUserId());
            if (profile == null)
            {
                profile = new UserProfile { UserId = GetUserId() };
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
