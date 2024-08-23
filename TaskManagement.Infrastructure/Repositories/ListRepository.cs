using MongoDB.Driver;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces.IRepositories;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories
{
	public class ListRepository : IListRepository
	{
		private readonly IMongoCollection<List> _lists;

		public ListRepository(MongoDbContext dbContext)
		{
			_lists = dbContext.Lists;
		}

		public async Task<List<List>> GetAllListsAsync()
		{
			return await _lists.Find(list => !list.IsDeleted).ToListAsync();
		}

		public async Task<List> GetListByIdAsync(string id)
		{
			return await _lists.Find(list => list.Id == id).FirstOrDefaultAsync();
		}

		public async Task<List> CreateListAsync(List list)
		{
			await _lists.InsertOneAsync(list);
			return list;
		}

		public async Task<bool> UpdateListAsync(List list)
		{
			var result = await _lists.ReplaceOneAsync(l => l.Id == list.Id, list);
			return result.IsAcknowledged && result.ModifiedCount > 0;
		}

		public async Task<bool> DeleteListAsync(string id)
		{
			var update = Builders<List>.Update.Set(g => g.IsDeleted, true);
			var result = await _lists.UpdateOneAsync(g => g.Id == id, update);
			return result.IsAcknowledged && result.ModifiedCount > 0;
		}


		public async Task<bool> ListExistsByNameAsync(string name)
		{
			var filter = Builders<List>.Filter.Eq(g => g.Name, name);
			var count = await _lists.CountDocumentsAsync(filter);
			return count > 0;
		}

		public async Task<bool> ListExistsByIdAsync(string id)
		{
			var filter = Builders<List>.Filter.Eq(g => g.Id, id);
			var count = await _lists.CountDocumentsAsync(filter);
			return count > 0;
		}
	}
}