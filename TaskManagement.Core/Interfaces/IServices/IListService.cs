using TaskManagement.Core.Commons.DTOs;
using TaskManagement.Core.Commons.Responses;

namespace TaskManagement.Core.Interfaces.IServices
{
	public interface IListService
	{
		Task<ServiceResponse<List<ResponseListDto>>> GetAllListsAsync();
		Task<ServiceResponse<ResponseListDto?>> GetListByIdAsync(string id);
		Task<ServiceResponse<ResponseListDto?>> CreateListAsync(RequestListDto listDto);
		Task<ServiceResponse<bool>> UpdateListAsync(RequestListDto listDto);
		Task<ServiceResponse<bool>> DeleteListAsync(string id);


	}
}