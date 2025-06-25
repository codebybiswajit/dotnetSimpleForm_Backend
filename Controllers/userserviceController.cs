using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model;
using WebApplication1.Services;
using WebApplication1.Middleware;
using System.Security.Claims;
using Microsoft.AspNetCore.HttpLogging;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserServiceController : ControllerBase
    {
        private readonly MongoDBService _mongoDBService;
        private readonly JwtAuthentication _jwtAuth;

        public UserServiceController(MongoDBService mongoDBService, JwtAuthentication jwtAuth)
        {
            _mongoDBService = mongoDBService;
            _jwtAuth = jwtAuth;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest login)
        {
            var user = await _mongoDBService.LoginAsync(login.UserName, login.Password);
            if (user == null) return NotFound("Invalid username or password");

            var token = _jwtAuth.Authenticate(user.UserName, user.Password, user.Id);

            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(3)
            });

            return Ok(new { message = "Logged in successfully", token, user });
        }
        [AllowAnonymous]
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser(User user)
        {
            await _mongoDBService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetCurrentUser), new { id = user.Id }, user);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var mongoId = User.FindFirstValue("mongoId");
            if (string.IsNullOrEmpty(mongoId)) return Unauthorized();

            var user = await _mongoDBService.GetUserByIdAsync(mongoId);
            return user == null ? NotFound() : Ok(user);
        }

        [Authorize]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] User input)
        {
            var mongoId = User.FindFirstValue("mongoId");
            if (string.IsNullOrEmpty(mongoId)) return Unauthorized();

            var user = await _mongoDBService.GetUserByIdAsync(mongoId);
            if (user == null) return NotFound();

            user.Name = input.Name ?? user.Name;
            user.Email = input.Email ?? user.Email;
            user.Password = input.Password ?? user.Password;
            user.Dob = input.Dob ?? user.Dob;

            await _mongoDBService.UpdateUserAsync(mongoId, user);
            return Ok("User updated successfully");
        }

        [Authorize]
        [HttpDelete("me")]
        public async Task<IActionResult> DeleteCurrentUser()
        {
            var mongoId = User.FindFirstValue("mongoId");
            if (string.IsNullOrEmpty(mongoId)) return Unauthorized();

            var user = await _mongoDBService.GetUserByIdAsync(mongoId);
            if (user == null) return NotFound();

            await _mongoDBService.DeleteUserAsync(mongoId);
            return Ok("Account deleted");
        }
        [Authorize]
        [HttpPatch("logout/me")]
        public async Task<IActionResult> LogOut()
        {
            User.FindFirstValue("mongoId");
            Response.Cookies.Delete("jwt");
            return Ok("Logout Successfully");
        }


        [HttpGet("get-all-user")]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _mongoDBService.GetUsersAsync());
        }

    }
}
