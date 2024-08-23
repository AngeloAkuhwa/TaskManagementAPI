using System.Text.Json.Serialization;

namespace TaskManagement.Core.Commons.DTOs
{
	public record RequestListDto(
		string Name,
		string Description,
		string GroupId,
		List<string> TaskIds,
		string CreatedBy,
		string UpdatedBy,
		[property: JsonIgnore] string? Id	= null
	);

	public record ResponseListDto(
		string Name,
		string Description,
		string GroupId,
		List<string> TaskIds,
		string CreatedBy,
		string UpdatedBy,
		string Id
	);
}