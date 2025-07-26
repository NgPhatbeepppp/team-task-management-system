// Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using TeamTaskManagementSystem.ViewModels;
using TeamTaskManagementSystem.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

using TeamTaskManagementSystem.Interfaces.IAuth_User;

using System.Linq;


namespace TeamTaskManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;
        private readonly IPasswordHasher<User> _hasher;

        public AuthController(IAuthService authService, IUserRepository userRepository, IConfiguration config)
        {
            _authService = authService;
            _userRepository = userRepository;
            _config = config;
            _hasher = new PasswordHasher<User>();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthRegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(request);

            if (!result.Success)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(new { token = result.Token });

        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthLoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = (await _userRepository.GetAllAsync()).FirstOrDefault(u => u.Username == request.Username);

            if (user == null || _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
                return Unauthorized("Sai tài khoản hoặc mật khẩu.");

            if (!user.IsActive)
                return Unauthorized("Tài khoản đã bị khóa.");

            var token = GenerateJwtToken(user);
            return Ok(new { token });
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
