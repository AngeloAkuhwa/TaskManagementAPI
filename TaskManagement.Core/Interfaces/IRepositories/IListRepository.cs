using TaskManagement.Core.Entities;

namespace TaskManagement.Core.Interfaces.IRepositories
{
	public interface IListRepository
	{
		Task<List<List>> GetAllListsAsync();
		Task<List> GetListByIdAsync(string id);
		Task<List> CreateListAsync(List list);
		Task<bool> UpdateListAsync(List list);
		Task<bool> DeleteListAsync(string id);
		Task<bool> ListExistsByNameAsync(string name);
		Task<bool> ListExistsByIdAsync(string name);
	}
}