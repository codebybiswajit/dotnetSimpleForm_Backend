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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string _id)
        {
            var user = await _mongoDBService.GetUserByIdAsync(_id);

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            await _mongoDBService.DeleteUserAsyncbyid(_id);
            return Ok(new { message = "User deleted successfully." });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var _id = User.FindFirstValue("mongoId");
            if (string.IsNullOrEmpty(_id)) return Unauthorized();

            var user = await _mongoDBService.GetUserByIdAsync(_id);
            return user == null ? NotFound() : Ok(user);
        }

        [Authorize]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] User input)
        {
            var _id = User.FindFirstValue("mongoId");
            if (string.IsNullOrEmpty(_id)) return Unauthorized();

            var user = await _mongoDBService.GetUserByIdAsync(_id);
            if (user == null) return NotFound();

            user.Name = input.Name ?? user.Name;
            user.Email = input.Email ?? user.Email;
            user.Password = input.Password ?? user.Password;
            user.Dob = input.Dob ?? user.Dob;

            await _mongoDBService.UpdateUserAsync(_id, user);
            return Ok("User updated successfully");
        }

        [Authorize]
        [HttpDelete("me")]
        public async Task<IActionResult> DeleteCurrentUser()
        {
            var _id = User.FindFirstValue("mongoId");
            if (string.IsNullOrEmpty(_id)) return Unauthorized();

            var user = await _mongoDBService.GetUserByIdAsync(_id);
            if (user == null) return NotFound();

            await _mongoDBService.DeleteUserAsync(_id);
            return Ok("Account deleted");
        }
        [Authorize]
        [HttpPatch("logout/me")]
        public async Task<IActionResult> LogOut()
        {
            var _id = User.FindFirstValue("mongoId");
            if (string.IsNullOrEmpty(_id)) return Unauthorized();

            var user = await _mongoDBService.GetUserByIdAsync(_id);
            if (user == null) return NotFound();
            Response.Cookies.Delete("jwt");
            return Ok("Logout Successfully");
        }


        [HttpGet("get-all-user")]
        public async Task<IActionResult> GetUsers(int startPage = 1, int limit = 2)
        {
            var users = await _mongoDBService.GetUsersAsync(startPage, limit);
            var totalCount = await _mongoDBService.TotalCount();
            return Ok(new
            {
                user = users,
                totalCount
            });
        }
        [HttpGet("get-all-user/ascending")]
        public async Task<IActionResult> GetUsersAscending(int startPage = 1, int limit = 2)
        {
            var users = await _mongoDBService.GetUsersAsync(startPage, limit);
            var totalCount = await _mongoDBService.TotalCount();
            var sortedUsers = users.OrderBy(user => user.Name);
            return Ok(new
            {
                user = sortedUsers,
                totalCount
            });
        }
        [HttpGet("get-all-user/descending")]
        public async Task<IActionResult> GetUsersDescending(int startPage = 1, int limit = 2)
        {
            var users = await _mongoDBService.GetUsersAsync(startPage, limit);
            var totalCount = await _mongoDBService.TotalCount();
            var sortedUsers = users.OrderByDescending(user => user.Name);
            return Ok(new
            {
                user = sortedUsers,
                totalCount
            }); ;
        }
        [HttpGet("get-all-user/created/descending")]
        public async Task<IActionResult> GetUsersCreateDescending(int startPage = 1, int limit = 2)
        {
            var users = await _mongoDBService.GetUsersAsync(startPage, limit);
            var totalCount = await _mongoDBService.TotalCount();
            var sortedUsers = users.OrderByDescending(user => user.CreatedAt);
            return Ok(new
            {
                user = sortedUsers,
                totalCount
            }); ;
        }
        [HttpGet("get-all-user/created/ascending")]
        public async Task<IActionResult> GetUsersCreateAscending(int startPage = 1, int limit = 2)
        {
            var users = await _mongoDBService.GetUsersAsync(startPage, limit);
            var totalCount = await _mongoDBService.TotalCount();
            var sortedUsers = users.OrderBy(user => user.CreatedAt);
            return Ok(new
            {
                user = sortedUsers,
                totalCount
            }); ;
        }
        [HttpGet("get-all-user/created/ascending/pagination")]
        public async Task<IActionResult> GetUsersCreateAscendingPagination(int startPage = 1)
        {

            // var sortedUsers = users.OrderBy(user => user.CreatedAt);
            return Ok(await _mongoDBService.GetUsersAsyncPagination(startPage));
        }

    }
}
