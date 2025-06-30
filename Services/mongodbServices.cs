using System.Security.Claims;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
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
        public async Task<User> LoginAsync(string username, string password)
        {
            return await _users.Find(u => u.UserName == username && u.Password == password).FirstOrDefaultAsync();
        }

        // public class userData
        // {
        //     public string Id { get; set; }
        //     public string Name { get; set; }
        //     public string Email { get; set; }

        // }
        public async Task<List<User>> GetUsersAsync(int startPage, int limit)
        {
            var users = await _users
                .Find(_ => true)
                .Project<User>(Builders<User>.Projection.Exclude(u => u.Password)
            .Exclude(u => u.ConfirmPassword))
                // .Project(user => new userData
                // {
                //     Id = user.Id,
                //     Name = user.Name,
                //     Email = user.Email
                // })
                .Skip((startPage - 1) * limit)
                .Limit(limit)
                .ToListAsync();

            return users;
        }

        public async Task<List<User>> GetUsersAsyncPagination(int startPage)
        {

            var pageSize = 3;
            return await _users.Find(_ => true)
                .Skip((startPage - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }
        public async Task<long> TotalCount()
        {
            var totalCount = await _users.CountDocumentsAsync(_ => true);
            return totalCount;
        }
        public async Task<User> GetUserByIdAsync(string id)
        {
            return await _users.Find(user => user.Id == id).FirstOrDefaultAsync();
        }


        public async Task CreateUserAsync(User user)
        {
            await _users.InsertOneAsync(user);
        }

        public async Task DeleteUserAsyncbyid(string id)
        {
            await _users.DeleteOneAsync(u => u.Id == id);
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
