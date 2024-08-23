using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using System.Net;
using TaskManagement.Core.Commons.DTOs;
using TaskManagement.Core.Commons.Mappers;
using TaskManagement.Core.Commons.Responses;
using TaskManagement.Core.Interfaces.IRepositories;
using TaskManagement.Core.Interfaces.IServices;
using TaskManagement.Infrastructure.Extensions;
using TaskManagement.Infrastructure.Helpers;
using TaskManagement.Infrastructure.Settings;

namespace TaskManagement.Infrastructure.Services
{
	public class TaskService : ITaskService
	{
		private readonly ITaskRepository _taskRepository;
		private readonly IDistributedCache _cache;
		private readonly RedisSettings _redisSettings;
		private readonly ILogger<TaskService> _logger;

		public TaskService(ITaskRepository taskRepository, IDistributedCache cache, IOptions<RedisSettings> redisSettings, ILogger<TaskService> logger)
		{
			_taskRepository = taskRepository;
			_cache = cache;
			_redisSettings = redisSettings.Value;
			_logger = logger;
		}

		public async Task<ServiceResponse<List<ResponseScheduleTaskDto>>> GetAllTasksAsync()
		{
			LoggingHelper.LogInformation(_logger, nameof(GetAllTasksAsync), "GetAllTasksAsync request operation started");

			try
			{
				var tasks = await _cache.GetOrSetAsync(CachedKeys.AllTasks, async () =>
				{
					var taskEntities = await _taskRepository.GetAllTasksAsync();
					return taskEntities.Select(t => t.ToDto()).ToList();
				}, _redisSettings);

				LoggingHelper.LogInformation(_logger, nameof(GetAllTasksAsync), "Tasks retrieved successfully");

				return new ServiceResponse<List<ResponseScheduleTaskDto>>()
				{
					Data = tasks.Any() ? tasks : Enumerable.Empty<ResponseScheduleTaskDto>().ToList(),
					Success = true,
					StatusCode = HttpStatusCode.OK,
					Message = tasks.Any() ? "Tasks retrieved successfully." : "No tasks found."
				};
			}
			catch (Exception ex)
			{
				LoggingHelper.LogError(_logger, nameof(GetAllTasksAsync), ex.Message);
				return new ServiceResponse<List<ResponseScheduleTaskDto>>()
				{
					Data = Enumerable.Empty<ResponseScheduleTaskDto>().ToList(),
					Success = false,
					StatusCode = HttpStatusCode.InternalServerError,
					Message = "An error occurred while retrieving the tasks."
				};
			}
		}

		public async Task<ServiceResponse<ResponseScheduleTaskDto?>> GetTaskByIdAsync(string id)
		{
			LoggingHelper.LogInformation(_logger, nameof(GetTaskByIdAsync), "GetTaskByIdAsync request operation started");

			try
			{
				if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
				{
					var message = string.IsNullOrEmpty(id) ? "Task ID cannot be null or empty." : "Invalid Task ID.";
					LoggingHelper.LogError(_logger, nameof(GetTaskByIdAsync), message);
					return new ServiceResponse<ResponseScheduleTaskDto?>()
					{
						Data = null,
						StatusCode = HttpStatusCode.BadRequest,
						Success = false,
						Message = message
					};
				}

				var task = await _cache.GetOrSetAsync(CachedKeys.TaskById(id), async () =>
				{
					if (!await _taskRepository.TaskExistsByIdAsync(id))
					{
						return null;
					}

					var taskEntity = await _taskRepository.GetTaskByIdAsync(id);
					return taskEntity?.ToDto();
				}, _redisSettings);

				if (task == null)
				{
					var notFoundMessage = $"Task with ID {id} not found.";
					LoggingHelper.LogError(_logger, nameof(GetTaskByIdAsync), notFoundMessage);
					return new ServiceResponse<ResponseScheduleTaskDto?>()
					{
						Data = null,
						StatusCode = HttpStatusCode.NotFound,
						Success = false,
						Message = notFoundMessage
					};
				}

				LoggingHelper.LogInformation(_logger, nameof(GetTaskByIdAsync), $"Task with ID '{id}' retrieved successfully");

				return new ServiceResponse<ResponseScheduleTaskDto?>()
				{
					Data = task,
					Success = true,
					StatusCode = HttpStatusCode.OK,
					Message = "Task retrieved successfully."
				};
			}
			catch (Exception ex)
			{
				LoggingHelper.LogError(_logger, nameof(GetTaskByIdAsync), ex.Message);
				return new ServiceResponse<ResponseScheduleTaskDto?>()
				{
					Data = null,
					Success = false,
					StatusCode = HttpStatusCode.InternalServerError,
					Message = "An error occurred while retrieving the task."
				};
			}
		}

		public async Task<ServiceResponse<ResponseScheduleTaskDto?>> CreateTaskAsync(RequestScheduleTaskDto taskDto)
		{
			LoggingHelper.LogInformation(_logger, nameof(CreateTaskAsync), "CreateTaskAsync request operation started");

			try
			{
				if (await _taskRepository.TaskExistsByNameAsync(taskDto.Title, taskDto.Description))
				{
					var conflictMessage = $"Task with title '{taskDto.Title}' and description '{taskDto.Description}' already exists.";
					LoggingHelper.LogError(_logger, nameof(CreateTaskAsync), conflictMessage);

					var existingTask = await _taskRepository.GetTaskByTitleAndDescriptionAsync(taskDto.Title, taskDto.Description);

					return new ServiceResponse<ResponseScheduleTaskDto>()
					{
						Data = existingTask.ToDto(),
						StatusCode = HttpStatusCode.Conflict,
						Success = false,
						Message = conflictMessage
					};
				}

				var task = taskDto.ToEntity();
				var createdTask = await _taskRepository.CreateTaskAsync(task);

				// Invalidate the cache for the list of all tasks
				await _cache.RemoveAsync(CachedKeys.AllTasks);

				LoggingHelper.LogInformation(_logger, nameof(CreateTaskAsync), $"Task '{taskDto.Title}' created successfully.");

				return new ServiceResponse<ResponseScheduleTaskDto?>()
				{
					Data = createdTask.ToDto(),
					StatusCode = HttpStatusCode.Created,
					Success = true,
					Message = "Task created successfully."
				};
			}
			catch (Exception ex)
			{
				LoggingHelper.LogError(_logger, nameof(CreateTaskAsync), ex.Message);
				return new ServiceResponse<ResponseScheduleTaskDto?>()
				{
					Data = null,
					StatusCode = HttpStatusCode.InternalServerError,
					Success = false,
					Message = "An error occurred while creating the task."
				};
			}
		}

		public async Task<ServiceResponse<bool>> UpdateTaskAsync(RequestScheduleTaskDto taskDto)
		{
			LoggingHelper.LogInformation(_logger, nameof(UpdateTaskAsync), "UpdateTaskAsync request operation started");

			try
			{
				if (string.IsNullOrEmpty(taskDto.Id) || !ObjectId.TryParse(taskDto.Id, out _))
				{
					var message = !ObjectId.TryParse(taskDto.Id, out _) ? "Invalid Task ID." : "Task ID cannot be null or empty.";
					LoggingHelper.LogError(_logger, nameof(UpdateTaskAsync), message);
					return new ServiceResponse<bool>()
					{
						Data = false,
						StatusCode = HttpStatusCode.BadRequest,
						Success = false,
						Message = message
					};
				}

				if (!await _taskRepository.TaskExistsByIdAsync(taskDto.Id))
				{
					var notFoundMessage = $"Task with ID {taskDto.Id} not found.";
					LoggingHelper.LogError(_logger, nameof(UpdateTaskAsync), notFoundMessage);
					return new ServiceResponse<bool>()
					{
						Data = false,
						StatusCode = HttpStatusCode.NotFound,
						Success = false,
						Message = notFoundMessage
					};
				}

				var updateResult = await _taskRepository.UpdateTaskAsync(taskDto.ToEntity());

				if (updateResult)
				{
					// Invalidate the cache for the updated task and the list of all tasks
					await _cache.RemoveAsync(CachedKeys.TaskById(taskDto.Id));
					await _cache.RemoveAsync(CachedKeys.AllTasks);

					LoggingHelper.LogInformation(_logger, nameof(UpdateTaskAsync), $"Task with ID '{taskDto.Id}' updated successfully.");
				}

				return new ServiceResponse<bool>()
				{
					Data = updateResult,
					Success = updateResult,
					StatusCode = updateResult ? HttpStatusCode.NoContent : HttpStatusCode.NotModified,
					Message = updateResult ? "Task updated successfully." : "No changes made to the task."
				};
			}
			catch (Exception ex)
			{
				LoggingHelper.LogError(_logger, nameof(UpdateTaskAsync), ex.Message);
				return new ServiceResponse<bool>()
				{
					Data = false,
					StatusCode = HttpStatusCode.InternalServerError,
					Success = false,
					Message = "An error occurred while updating the task."
				};
			}
		}

		public async Task<ServiceResponse<bool>> DeleteTaskAsync(string id)
		{
			LoggingHelper.LogInformation(_logger, nameof(DeleteTaskAsync), "DeleteTaskAsync request operation started");

			try
			{
				if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
				{
					var message = string.IsNullOrEmpty(id) ? "Task ID cannot be null or empty." : "Invalid Task ID.";
					LoggingHelper.LogError(_logger, nameof(DeleteTaskAsync), message);
					return new ServiceResponse<bool>()
					{
						Data = false,
						StatusCode = HttpStatusCode.BadRequest,
						Success = false,
						Message = message
					};
				}

				if (!await _taskRepository.TaskExistsByIdAsync(id))
				{
					var notFoundMessage = $"Task with ID {id} not found.";
					LoggingHelper.LogError(_logger, nameof(DeleteTaskAsync), notFoundMessage);
					return new ServiceResponse<bool>()
					{
						Data = false,
						StatusCode = HttpStatusCode.NotFound,
						Success = false,
						Message = notFoundMessage
					};
				}

				var deleteResult = await _taskRepository.DeleteTaskAsync(id);

				if (deleteResult)
				{
					// Invalidate the cache for the deleted task and the list of all tasks
					await _cache.RemoveAsync(CachedKeys.TaskById(id));
					await _cache.RemoveAsync(CachedKeys.AllTasks);

					LoggingHelper.LogInformation(_logger, nameof(DeleteTaskAsync), $"Task with ID '{id}' deleted successfully.");
				}

				return new ServiceResponse<bool>()
				{
					Data = deleteResult,
					StatusCode = deleteResult ? HttpStatusCode.NoContent : HttpStatusCode.InternalServerError,
					Success = deleteResult,
					Message = deleteResult ? "Task deleted successfully." : "Error occurred while deleting the task."
				};
			}
			catch (Exception ex)
			{
				LoggingHelper.LogError(_logger, nameof(DeleteTaskAsync), ex.Message);
				return new ServiceResponse<bool>()
				{
					Data = false,
					StatusCode = HttpStatusCode.InternalServerError,
					Success = false,
					Message = "An error occurred while deleting the task."
				};
			}
		}
	}
}
