using System.Text.Json.Serialization;

namespace TaskManagement.Core.Commons.DTOs
{
	public record GroupRequestDto(
		string Name,
		string Description,
		List<string> ListIds,
		string CreatedBy,
		string UpdatedBy,
		[property: JsonIgnore] string? Id = null
	);

	public record GroupResponseDto(
		string Id,
		string Name,
		string Description,
		List<string> ListIds,
		string CreatedBy,
		string UpdatedBy
	);
}