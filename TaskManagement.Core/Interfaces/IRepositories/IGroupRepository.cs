using TaskManagement.Core.Entities;

namespace TaskManagement.Core.Interfaces.IRepositories
{
	public interface IGroupRepository
	{
		Task<List<Group>> GetAllGroupsAsync();
		Task<Group> GetGroupByIdAsync(string id);
		Task<Group> CreateGroupAsync(Group group);
		Task<bool> UpdateGroupAsync(Group group);
		Task<bool> DeleteGroupAsync(string id);
		Task<bool> GroupExistsByNameAsync(string name);
		Task<bool> GroupExistsByIdAsync(string name);
	}
}