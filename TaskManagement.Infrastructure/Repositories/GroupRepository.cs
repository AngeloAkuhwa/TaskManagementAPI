using MongoDB.Driver;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces.IRepositories;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories
{
	public class GroupRepository : IGroupRepository
	{
		private readonly IMongoCollection<Group> _groups;

		public GroupRepository(MongoDbContext dbContext)
		{
			_groups = dbContext.Groups;
		}

		public async Task<List<Group>> GetAllGroupsAsync()
		{
			return await _groups.Find(group => !group.IsDeleted).ToListAsync();
		}

		public async Task<Group> GetGroupByIdAsync(string id)
		{
			return await _groups.Find(group => group.Id == id).FirstOrDefaultAsync();
		}

		public async Task<Group> CreateGroupAsync(Group group)
		{
			await _groups.InsertOneAsync(group);
			return group;
		}

		public async Task<bool> UpdateGroupAsync(Group group)
		{
			var result = await _groups.ReplaceOneAsync(g => g.Id == group.Id, group);
			return result.IsAcknowledged && result.ModifiedCount > 0;
		}

		public async Task<bool> DeleteGroupAsync(string id)
		{
			var update = Builders<Group>.Update.Set(g => g.IsDeleted, true);
			var result = await _groups.UpdateOneAsync(g => g.Id == id, update);
			return result.IsAcknowledged && result.ModifiedCount > 0;
		}

		public async Task<bool> GroupExistsByNameAsync(string name)
		{
			var filter = Builders<Group>.Filter.Eq(g => g.Name, name);
			var count = await _groups.CountDocumentsAsync(filter);
			return count > 0;
		}

		public async Task<bool> GroupExistsByIdAsync(string id)
		{
			var filter = Builders<Group>.Filter.Eq(g => g.Id, id);
			var count = await _groups.CountDocumentsAsync(filter);
			return count > 0;
		}
	}
}