using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model;
using WebApplication1.Services;
using WebApplication1.Middleware;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserServiceController : ControllerBase
    {
        private readonly MongoDBService _mongoDBService;

        public UserServiceController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            var users = await _mongoDBService.GetUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id:length(24)}", Name = "GetUserById")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            var user = await _mongoDBService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound($"User with ID = {id} not found");

            return Ok(user);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest login)
        {
            var user = await _mongoDBService.LoginAsync(login.UserName, login.Password);
            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }

            return Ok(user);
        }

        [HttpPost("create", Name = "CreateUser")]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            await _mongoDBService.CreateUserAsync(user);
            return CreatedAtRoute("GetUserById", new { id = user.Id }, user);
        }


        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> UpdateUser(string id, User user)
        {
            var existingUser = await _mongoDBService.GetUserByIdAsync(id);
            if (existingUser == null)
                return NotFound($"User with ID = {id} not found");

            var updatedUser = new User
            {
                Id = existingUser.Id,
                Name = string.IsNullOrWhiteSpace(user.Name) ? existingUser.Name : user.Name,
                Email = string.IsNullOrWhiteSpace(user.Email) ? existingUser.Email : user.Email,
                Password = string.IsNullOrWhiteSpace(user.Password) ? existingUser.Password : user.Password,
                Dob = string.IsNullOrWhiteSpace(user.Dob) ? existingUser.Dob : user.Dob
            };

            await _mongoDBService.UpdateUserAsync(id, updatedUser);
            return NoContent();
        }


        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _mongoDBService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound($"User with ID = {id} not found");

            await _mongoDBService.DeleteUserAsync(id);
            return Ok($"Deleted user with ID = {id}");
        }
    }
}
