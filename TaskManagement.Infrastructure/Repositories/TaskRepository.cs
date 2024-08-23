using MongoDB.Driver;
using System.Threading.Tasks;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces.IRepositories;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories
{
	public class TaskRepository : ITaskRepository
	{
		private readonly IMongoCollection<ScheduleTask> _tasks;

		public TaskRepository(MongoDbContext context)
		{
			_tasks = context.ScheduleTasks;
		}

		public async Task<List<ScheduleTask>> GetAllTasksAsync()
		{
			return await _tasks.Find(task => true).ToListAsync();
		}

		public async Task<ScheduleTask> GetTaskByIdAsync(string id)
		{
			return await _tasks.Find(task => task.Id == id).FirstOrDefaultAsync();
		}

		public async Task<ScheduleTask> CreateTaskAsync(ScheduleTask task)
		{
			await _tasks.InsertOneAsync(task);
			return task;
		}

		public async Task<bool> UpdateTaskAsync(ScheduleTask task)
		{
			var result = await _tasks.ReplaceOneAsync(t => t.Id == task.Id, task);
			return result.IsAcknowledged && result.ModifiedCount > 0;
		}

		public async Task<bool> DeleteTaskAsync(string id)
		{
			var update = Builders<ScheduleTask>.Update.Set(g => g.IsDeleted, true);
			var result = await _tasks.UpdateOneAsync(g => g.Id == id, update);
			return result.IsAcknowledged && result.ModifiedCount > 0;
		}

		public async Task<bool> TaskExistsByNameAsync(string title, string description)
		{
			var filter = Builders<ScheduleTask>.Filter.And(
				Builders<ScheduleTask>.Filter.Eq(t => t.Title, title),
				Builders<ScheduleTask>.Filter.Eq(t => t.Description, description)
			);

			var count = await _tasks.CountDocumentsAsync(filter);
			return count > 0;
		}

		public async Task<bool> TaskExistsByIdAsync(string id)
		{
			var filter = Builders<ScheduleTask>.Filter.Eq(g => g.Id, id);
			var count = await _tasks.CountDocumentsAsync(filter);
			return count > 0;
		}

		public async Task<ScheduleTask?> GetTaskByTitleAndDescriptionAsync(string title, string description)
		{
			var filter = Builders<ScheduleTask>.Filter.And(
				Builders<ScheduleTask>.Filter.Eq(t => t.Title, title),
				Builders<ScheduleTask>.Filter.Eq(t => t.Description, description)
			);

			var task = await _tasks.Find(filter).FirstOrDefaultAsync();

			return task;
		}
	}
}