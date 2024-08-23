using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using TaskManagement.Core.Commons.DTOs;
using TaskManagement.Core.Commons.Enums;
using TaskManagement.Core.Commons.Mappers;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces.IRepositories;
using TaskManagement.Infrastructure.Services;
using TaskManagement.Infrastructure.Settings;
using Xunit;

namespace TaskManagement.Tests
{
	public class TaskServiceTests
	{
		private readonly Mock<ITaskRepository> _taskRepositoryMock;
		private readonly TaskService _taskService;

		public TaskServiceTests()
		{
			_taskRepositoryMock = new Mock<ITaskRepository>();
			Mock<IDistributedCache> cacheMock = new();
			Mock<IOptions<RedisSettings>> redisSettingsMock = new();
			Mock<ILogger<TaskService>> loggerTaskService = new();

		var redisSettings = new RedisSettings { SlidingExpiration = TimeSpan.FromMinutes(30), AbsoluteExpiration = TimeSpan.FromHours(1) };
			redisSettingsMock.Setup(rs => rs.Value).Returns(redisSettings);

			_taskService = new TaskService(_taskRepositoryMock.Object, cacheMock.Object, redisSettingsMock.Object, loggerTaskService.Object);
		}

		[Fact]
		public async Task GetAllTasksAsync_ShouldReturnTasks_WhenTasksExist()
		{
			// Arrange
			var tasks = new List<ResponseScheduleTaskDto>
			{
				new ResponseScheduleTaskDto("Title1", "Description1", "Pending", 1, "66c5b64f643f79690b1a7d03",
					"66c5b64f643f79690b1a7d04", new List<string>(), "CreatedBy1", "UpdatedBy1", false,
					"66c5b64f643f79690b1a7d01"),
				new ResponseScheduleTaskDto("Title2", "Description2", "Completed", 2, "66c5b64f643f79690b1a7d05",
					"66c5b64f643f79690b1a7d06", new List<string>(), "CreatedBy2", "UpdatedBy2", false, "66c5b64f643f79690b1a7d02")
			};

			_taskRepositoryMock.Setup(repo => repo.GetAllTasksAsync())
					.ReturnsAsync(tasks.Select(t => t.ToEntity()).ToList());

			// Act
			var result = await _taskService.GetAllTasksAsync();

			// Assert
			Assert.True(result.Success);
			Assert.Equal(2, result.Data.Count);
			Assert.Equal("Tasks retrieved successfully.", result.Message);
		}

		[Fact]
		public async Task GetTaskByIdAsync_ShouldReturnTask_WhenTaskExists()
		{
			// Arrange
			var taskId = "66c5b64f643f79690b1a7d01";
			var task = new ResponseScheduleTaskDto("Title1", "Description1", "Pending", (int)TaskPriority.Medium, "66c5b64f643f79690b1a7d03", "66c5b64f643f79690b1a7d04", new List<string>(), "CreatedBy1", "UpdatedBy1", false, taskId);

			_taskRepositoryMock.Setup(repo => repo.GetTaskByIdAsync(taskId))
					.ReturnsAsync(task.ToEntity());
			_taskRepositoryMock.Setup(repo => repo.TaskExistsByIdAsync(taskId))
					.ReturnsAsync(true);

			// Act
			var result = await _taskService.GetTaskByIdAsync(taskId);

			// Assert
			Assert.True(result.Success);
			Assert.Equal(taskId, result.Data?.Id);
			Assert.Equal("Task retrieved successfully.", result.Message);
		}

		[Fact]
		public async Task CreateTaskAsync_ShouldCreateTask_WhenTaskIsValid()
		{
			// Arrange
			var taskDto = new RequestScheduleTaskDto("Title1", "Description1", "Pending", 1, "66c5b64f643f79690b1a7d03", "66c5b64f643f79690b1a7d04", new List<string>(), "CreatedBy1", "UpdatedBy1", false);
			var taskEntity = taskDto.ToEntity();
			var taskResponseDto = taskEntity.ToDto();

			_taskRepositoryMock.Setup(repo => repo.TaskExistsByNameAsync(taskDto.Title, taskDto.Description))
					.ReturnsAsync(false);
			_taskRepositoryMock.Setup(repo => repo.CreateTaskAsync(It.IsAny<TaskManagement.Core.Entities.ScheduleTask>()))
					.ReturnsAsync(taskEntity);

			// Act
			var result = await _taskService.CreateTaskAsync(taskDto);

			// Assert
			Assert.True(result.Success);
			Assert.Equal("Task created successfully.", result.Message);
			Assert.Equal(taskDto.Title, result.Data?.Title);
		}

		[Fact]
		public async Task UpdateTaskAsync_ShouldReturnNoContent_WhenUpdateIsSuccessful()
		{
			// Arrange
			var taskDto = new RequestScheduleTaskDto("Title1", "Description1", "Pending", 1, "66c5b64f643f79690b1a7d03", "66c5b64f643f79690b1a7d04", new List<string>(), "CreatedBy1", "UpdatedBy1", false, "66c5b64f643f79690b1a7d01");

			_taskRepositoryMock.Setup(repo => repo.TaskExistsByIdAsync(It.IsAny<string>()))
					.ReturnsAsync(true);
			_taskRepositoryMock.Setup(repo => repo.UpdateTaskAsync(It.IsAny<ScheduleTask>()))
				.ReturnsAsync(true);


			// Act
			var result = await _taskService.UpdateTaskAsync(taskDto);

			// Assert
			Assert.True(result.Success);
			Assert.Equal("Task updated successfully.", result.Message);
		}

		[Fact]
		public async Task DeleteTaskAsync_ShouldReturnNoContent_WhenDeleteIsSuccessful()
		{
			// Arrange
			var taskId = "66c5b64f643f79690b1a7d01";
			_taskRepositoryMock.Setup(repo => repo.TaskExistsByIdAsync(taskId))
					.ReturnsAsync(true);
			_taskRepositoryMock.Setup(repo => repo.DeleteTaskAsync(taskId))
					.ReturnsAsync(true);

			// Act
			var result = await _taskService.DeleteTaskAsync(taskId);

			// Assert
			Assert.True(result.Success);
			Assert.Equal("Task deleted successfully.", result.Message);
		}
	}
}
