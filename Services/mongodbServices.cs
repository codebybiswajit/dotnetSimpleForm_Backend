using MongoDB.Driver;
using WebApplication1.Model;

namespace WebApplication1.Services
{
    public class MongoDBService
    {
        private readonly IMongoCollection<User> _users;

        public MongoDBService(IMongoClient client)
        {
            var database = client.GetDatabase("admin");
            _users = database.GetCollection<User>("Users");
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _users.Find(_ => true).ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            return await _users.Find(user => user.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateUserAsync(User user)
        {
            await _users.InsertOneAsync(user);
        }

        public async Task UpdateUserAsync(string id, User user)
        {
            await _users.ReplaceOneAsync(u => u.Id == id, user);
        }

        public async Task DeleteUserAsync(string id)
        {
            await _users.DeleteOneAsync(u => u.Id == id);
        }
    }
}
