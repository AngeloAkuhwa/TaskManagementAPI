using Microsoft.AspNetCore.Mvc;
using TaskManagement.Core.Commons.DTOs;
using TaskManagement.Core.Interfaces.IServices;

namespace TaskManagmentAPI.Presentation.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class GroupsController : ControllerBase
	{
		private readonly IGroupService _groupService;

		public GroupsController(IGroupService groupService)
		{
			_groupService = groupService;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllGroups()
		{
			var groups = await _groupService.GetAllGroupsAsync();
			return Ok(groups);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetGroupById(string id)
		{
			var group = await _groupService.GetGroupByIdAsync(id);
			return Ok(group);
		}

		[HttpPost]
		public async Task<IActionResult> CreateGroup([FromBody] GroupRequestDto groupDto)
		{
			var createdGroup = await _groupService.CreateGroupAsync(groupDto);
			
			return createdGroup.Success ? CreatedAtAction(nameof(GetGroupById), new {id = createdGroup.Data?.Id}, createdGroup.Data) : BadRequest(createdGroup);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateGroup(string id, [FromBody] GroupRequestDto groupDto)
		{
			var result = await _groupService.UpdateGroupAsync(groupDto with {Id = id});
			return result.Success ? NoContent() : NotFound();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteGroup(string id)
		{
			var result = await _groupService.DeleteGroupAsync(id);
			return result.Success ? NoContent() : NotFound();
		}
	}
}