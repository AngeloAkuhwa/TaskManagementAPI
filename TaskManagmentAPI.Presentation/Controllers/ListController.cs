using Microsoft.AspNetCore.Mvc;
using TaskManagement.Core.Commons.DTOs;
using TaskManagement.Core.Interfaces.IServices;

namespace TaskManagmentAPI.Presentation.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ListsController : ControllerBase
	{
		private readonly IListService _listService;

		public ListsController(IListService listService)
		{
			_listService = listService;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllLists()
		{
			var lists = await _listService.GetAllListsAsync();
			return Ok(lists);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetListById(string id)
		{
			var list = await _listService.GetListByIdAsync(id);
			return Ok(list);
		}

		[HttpPost]
		public async Task<IActionResult> CreateList([FromBody] RequestListDto listDto)
		{
			var createdList = await _listService.CreateListAsync(listDto);
			return createdList.Success ? CreatedAtAction(nameof(GetListById), new { id = createdList.Data?.Id }, createdList) : BadRequest(createdList);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateList(string id, [FromBody] RequestListDto listDto)
		{
			var result = await _listService.UpdateListAsync(listDto with { Id = id });
			return result.Success ? NoContent() : NotFound();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteList(string id)
		{
			var result = await _listService.DeleteListAsync(id);
			return result.Success ? NoContent() : NotFound();
		}
	}
}