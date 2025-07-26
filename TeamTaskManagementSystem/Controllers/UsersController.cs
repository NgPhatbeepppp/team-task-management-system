using Microsoft.AspNetCore.Mvc;
using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Interfaces.IAuth_User;
using Microsoft.AspNetCore.Authorization;

namespace TeamTaskManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // GET: api/users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<User>>> SearchUsers([FromQuery] string query)
        {
            if (string.IsNullOrEmpty(query) || query.Length < 3)
            {
                // Chỉ tìm kiếm khi có ít nhất 3 ký tự để tránh quá tải
                return Ok(Enumerable.Empty<User>());
            }
            var users = await _userService.SearchUsersAsync(query);
            return Ok(users);
        }

        // POST: api/users
        [HttpPost]
        public async Task<ActionResult> Create(User user)
        {
            var success = await _userService.CreateUserAsync(user);
            if (!success) return BadRequest("Không thể tạo người dùng.");
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        // PUT: api/users/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, User user)
        {
            if (id != user.Id) return BadRequest("Id không khớp.");
            var success = await _userService.UpdateUserAsync(user);
            if (!success) return NotFound();
            return NoContent();
        }

        // DELETE: api/users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var success = await _userService.DeleteUserAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
