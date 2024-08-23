using Microsoft.AspNetCore.Mvc;
using TaskManagement.Core.Commons.DTOs;
using TaskManagement.Core.Interfaces.IServices;

namespace TaskManagmentAPI.Presentation.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class TasksController : ControllerBase
	{
		private readonly ITaskService _taskService;

		public TasksController(ITaskService taskService)
		{
			_taskService = taskService;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllTasks()
		{
			var tasks = await _taskService.GetAllTasksAsync();
			return Ok(tasks);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetTaskById(string id)
		{
			var task = await _taskService.GetTaskByIdAsync(id);
			return Ok(task);
		}

		[HttpPost]
		public async Task<IActionResult> CreateTask([FromBody] RequestScheduleTaskDto taskDto)
		{
			var createdTask = await _taskService.CreateTaskAsync(taskDto);
			return createdTask.Success ?  CreatedAtAction(nameof(GetTaskById), new { id = createdTask.Data.Id }, createdTask) : BadRequest(createdTask);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateTask(string id, [FromBody] RequestScheduleTaskDto taskDto)
		{
			var result = await _taskService.UpdateTaskAsync(taskDto with { Id = id });
			return result.Success ? NoContent() : NotFound();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteTask(string id)
		{
			var result = await _taskService.DeleteTaskAsync(id);
			return result.Success ? NoContent() : NotFound();
		}
	}
}
