using MongoDB.Driver;
using System.Threading.Tasks;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces.IRepositories;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly IMongoCollection<User> _users;

		public UserRepository(MongoDbContext dbContext)
		{
			_users = dbContext.Users;
		}

		public async Task<List<User>> GetAllUsersAsync()
		{
			return await _users.Find(user => !user.IsDeleted).ToListAsync();
		}

		public async Task<User> GetUserByIdAsync(string id)
		{
			return await _users.Find(user => user.Id == id).FirstOrDefaultAsync();
		}

		public async Task<User> CreateUserAsync(User user)
		{
			await _users.InsertOneAsync(user);
			return user;
		}

		public async Task<bool> UpdateUserAsync(User user)
		{
			var result = await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
			return result.IsAcknowledged && result.ModifiedCount > 0;
		}

		public async Task<bool> DeleteUserAsync(string id)
		{
			var update = Builders<User>.Update.Set(g => g.IsDeleted, true);
			var result = await _users.UpdateOneAsync(g => g.Id == id, update);
			return result.IsAcknowledged && result.ModifiedCount > 0;
		}

		public async Task<bool> UserExistsByEmailAsync(string email)
		{
			var filter = Builders<User>.Filter.Eq(t => t.Email, email);
				var count = await _users.CountDocumentsAsync(filter);
			return count > 0;
		}

		public async Task<bool> UserExistsByIdAsync(string id)
		{
			var filter = Builders<User>.Filter.Eq(g => g.Id, id);
			var count = await _users.CountDocumentsAsync(filter);
			return count > 0;
		}
	}
}