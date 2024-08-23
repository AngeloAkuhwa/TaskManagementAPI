using TaskManagement.Core.Entities;

namespace TaskManagement.Core.Interfaces.IRepositories
{
	public interface IUserRepository
	{
		Task<List<User>> GetAllUsersAsync();
		Task<User> GetUserByIdAsync(string id);
		Task<User> CreateUserAsync(User user);
		Task<bool> UpdateUserAsync(User user);
		Task<bool> DeleteUserAsync(string id);
		Task<bool> UserExistsByIdAsync(string id);
		Task<bool> UserExistsByEmailAsync(string email);
	}
}