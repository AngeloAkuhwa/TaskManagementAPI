using TaskManagement.Core.Entities;

namespace TaskManagement.Core.Interfaces.IRepositories
{
	public interface ITaskRepository
	{
		Task<List<ScheduleTask>> GetAllTasksAsync();
		Task<ScheduleTask> GetTaskByIdAsync(string id);
		Task<ScheduleTask> CreateTaskAsync(ScheduleTask task);
		Task<bool> UpdateTaskAsync(ScheduleTask task);
		Task<bool> DeleteTaskAsync(string id);

		Task<bool> TaskExistsByNameAsync(string title, string description);

		Task<bool> TaskExistsByIdAsync(string id);

		Task<ScheduleTask?> GetTaskByTitleAndDescriptionAsync(string title, string description);
	}
}