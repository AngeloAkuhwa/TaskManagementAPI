using TaskManagement.Core.Commons.DTOs;
using TaskManagement.Core.Commons.Responses;

namespace TaskManagement.Core.Interfaces.IServices
{
	public interface IGroupService
	{
		Task<ServiceResponse<List<GroupResponseDto>>> GetAllGroupsAsync();
		Task<ServiceResponse<GroupResponseDto?>> GetGroupByIdAsync(string id);
		Task<ServiceResponse<GroupResponseDto?>> CreateGroupAsync(GroupRequestDto groupDto);
		Task<ServiceResponse<bool>> UpdateGroupAsync(GroupRequestDto groupDto);
		Task<ServiceResponse<bool>> DeleteGroupAsync(string id);
	}
}