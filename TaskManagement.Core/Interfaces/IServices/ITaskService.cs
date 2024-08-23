using TaskManagement.Core.Commons.DTOs;
using TaskManagement.Core.Commons.Responses;

namespace TaskManagement.Core.Interfaces.IServices
{
	public interface ITaskService
	{
		Task<ServiceResponse<List<ResponseScheduleTaskDto>>> GetAllTasksAsync();
		Task<ServiceResponse<ResponseScheduleTaskDto?>> GetTaskByIdAsync(string id);
		Task<ServiceResponse<ResponseScheduleTaskDto?>> CreateTaskAsync(RequestScheduleTaskDto taskDto);
		Task<ServiceResponse<bool>> UpdateTaskAsync(RequestScheduleTaskDto taskDto);
		Task<ServiceResponse<bool>> DeleteTaskAsync(string id);
	}
}