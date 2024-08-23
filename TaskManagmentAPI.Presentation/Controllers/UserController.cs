using Microsoft.AspNetCore.Mvc;
using TaskManagement.Core.Commons.DTOs;
using TaskManagement.Core.Interfaces.IServices;

namespace TaskManagmentAPI.Presentation.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class UserController : ControllerBase
	{
		private readonly IUserService _userService;

		public UserController(IUserService userService)
		{
			_userService = userService;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllUsers()
		{
			var users = await _userService.GetAllUsersAsync();
			return Ok(users);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetUserById(string id)
		{
			var user = await _userService.GetUserByIdAsync(id);
			return Ok(user);
		}

		[HttpPost]
		public async Task<IActionResult> CreateUser([FromBody] RequestUserDto userDto)
		{
			var createdUser = await _userService.CreateUserAsync(userDto);
			return createdUser.Success ? CreatedAtAction(nameof(GetUserById), new { id = createdUser.Data.Id }, createdUser) : BadRequest(createdUser);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateUser(string id, [FromBody] RequestUserDto userDto)
		{
			var result = await _userService.UpdateUserAsync(userDto with { Id = id });
			return result.Success ? NoContent() : NotFound();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteUser(string id)
		{
			var result = await _userService.DeleteUserAsync(id);
			return result.Success ? NoContent() : NotFound();
		}
	}
}