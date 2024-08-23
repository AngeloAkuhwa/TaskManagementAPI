using TaskManagement.Core.Commons.DTOs;
using TaskManagement.Core.Commons.Responses;

namespace TaskManagement.Core.Interfaces.IServices
{
	public interface IUserService
	{
		Task<ServiceResponse<List<ResponseUserDto>>> GetAllUsersAsync();
		Task<ServiceResponse<ResponseUserDto?>> GetUserByIdAsync(string id);
		Task<ServiceResponse<ResponseUserDto?>> CreateUserAsync(RequestUserDto userDto);
		Task<ServiceResponse<bool>> UpdateUserAsync(RequestUserDto userDto);
		Task<ServiceResponse<bool>> DeleteUserAsync(string id);
	}
}