using System.Text.Json.Serialization;

namespace TaskManagement.Core.Commons.DTOs
{
	public record RequestScheduleTaskDto(
		string Title,
		string Description,
		string Status,
		int Priority,
		string ListId,
		string GroupId,
		List<string> AssignedUserIds,
		string CreatedBy,
		string UpdatedBy,
		bool IsDeleted,
		[property: JsonIgnore] string? Id = null
	);

	public record ResponseScheduleTaskDto(
		string Title,
		string Description,
		string Status,
		int Priority,
		string ListId,
		string GroupId,
		List<string> AssignedUserIds,
		string CreatedBy,
		string UpdatedBy,
		bool IsDeleted,
		string Id
	);
}