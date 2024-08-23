using System.Text.Json.Serialization;

namespace TaskManagement.Core.Commons.DTOs
{
	public record RequestUserDto(
		string Username,
		string Email,
		string FirstName,
		string LastName,
		List<string> AssignedTaskIds,
		[property: JsonIgnore] string? Id = null
	);

	public record ResponseUserDto(
		string Username,
		string Email,
		string FirstName,
		string LastName,
		List<string> AssignedTaskIds,
		string Id
	);
}