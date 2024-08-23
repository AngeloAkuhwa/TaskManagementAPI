using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Bson;
using System.Net;
using Microsoft.Extensions.Options;
using TaskManagement.Core.Commons.DTOs;
using TaskManagement.Core.Commons.Mappers;
using TaskManagement.Core.Commons.Responses;
using TaskManagement.Core.Interfaces.IRepositories;
using TaskManagement.Core.Interfaces.IServices;
using TaskManagement.Infrastructure.Extensions;
using TaskManagement.Infrastructure.Settings;

namespace TaskManagement.Infrastructure.Services
{
	public class UserService : IUserService
	{
		private readonly IUserRepository _userRepository;
		private readonly IDistributedCache _cache;
		private readonly RedisSettings _redisSettings;

		public UserService(IUserRepository userRepository, IDistributedCache cache, IOptions<RedisSettings> redisSettings)
		{
			_userRepository = userRepository;
			_cache = cache;
			_redisSettings = redisSettings.Value;
		}

		public async Task<ServiceResponse<List<ResponseUserDto>>> GetAllUsersAsync()
		{
			var users = await _cache.GetOrSetAsync(CachedKeys.AllUsers, async () =>
			{
				var userEntities = await _userRepository.GetAllUsersAsync();
				return userEntities.Select(u => u.ToDto()).ToList();
			}, _redisSettings);

			return new ServiceResponse<List<ResponseUserDto>>()
			{
				Data = users.Any() ? users : Enumerable.Empty<ResponseUserDto>().ToList(),
				Success = true,
				StatusCode = HttpStatusCode.OK,
				Message = users.Any() ? "Users retrieved successfully." : "No users found."
			};
		}

		public async Task<ServiceResponse<ResponseUserDto?>> GetUserByIdAsync(string id)
		{
			if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
			{
				return new ServiceResponse<ResponseUserDto?>()
				{
					Data = null,
					StatusCode = HttpStatusCode.BadRequest,
					Success = false,
					Message = string.IsNullOrEmpty(id) ? "User ID cannot be null or empty." : "Invalid User ID."
				};
			}

			var user = await _cache.GetOrSetAsync(CachedKeys.UserById(id), async () =>
			{
				if (!await _userRepository.UserExistsByIdAsync(id))
				{
					return null;
				}

				var userEntity = await _userRepository.GetUserByIdAsync(id);
				return userEntity?.ToDto();
			}, _redisSettings);

			if (user == null)
			{
				return new ServiceResponse<ResponseUserDto?>()
				{
					Data = null,
					StatusCode = HttpStatusCode.NotFound,
					Success = false,
					Message = $"User with ID {id} not found."
				};
			}

			return new ServiceResponse<ResponseUserDto?>()
			{
				Data = user,
				Success = true,
				StatusCode = HttpStatusCode.OK,
				Message = "User retrieved successfully."
			};
		}

		public async Task<ServiceResponse<ResponseUserDto?>> CreateUserAsync(RequestUserDto userDto)
		{
			if (string.IsNullOrWhiteSpace(userDto.Username) || string.IsNullOrWhiteSpace(userDto.Email))
			{
				return new ServiceResponse<ResponseUserDto?>()
				{
					Data = null,
					StatusCode = HttpStatusCode.BadRequest,
					Success = false,
					Message = "Username and Email cannot be null or empty."
				};
			}

			if (await _userRepository.UserExistsByEmailAsync(userDto.Email))
			{
				return new ServiceResponse<ResponseUserDto?>()
				{
					Data = null,
					StatusCode = HttpStatusCode.Conflict,
					Success = false,
					Message = $"User with Email {userDto.Email} already exists."
				};
			}

			var user = userDto.ToEntity();
			var createdUser = await _userRepository.CreateUserAsync(user);

			// Invalidate the cache for the list of all users
			await _cache.RemoveAsync(CachedKeys.AllUsers);

			return new ServiceResponse<ResponseUserDto?>()
			{
				Data = createdUser.ToDto(),
				StatusCode = HttpStatusCode.Created,
				Success = true,
				Message = "User created successfully."
			};
		}

		public async Task<ServiceResponse<bool>> UpdateUserAsync(RequestUserDto userDto)
		{
			if (string.IsNullOrWhiteSpace(userDto.Id) || !ObjectId.TryParse(userDto.Id, out _))
			{
				return new ServiceResponse<bool>()
				{
					Data = false,
					StatusCode = HttpStatusCode.BadRequest,
					Success = false,
					Message = !ObjectId.TryParse(userDto.Id, out _) ? "Invalid User ID." : "User ID cannot be null or empty."
				};
			}

			if (!await _userRepository.UserExistsByIdAsync(userDto.Id))
			{
				return new ServiceResponse<bool>()
				{
					Data = false,
					StatusCode = HttpStatusCode.NotFound,
					Success = false,
					Message = $"User with ID {userDto.Id} not found."
				};
			}

			var user = userDto.ToEntity();
			var updateResult = await _userRepository.UpdateUserAsync(user);

			if (updateResult)
			{
				// Invalidate the cache for the updated user and the list of all users
				await _cache.RemoveAsync(CachedKeys.UserById(userDto.Id));
				await _cache.RemoveAsync(CachedKeys.AllUsers);
			}

			return new ServiceResponse<bool>()
			{
				Data = updateResult,
				Success = updateResult,
				StatusCode = updateResult ? HttpStatusCode.NoContent : HttpStatusCode.NotModified,
				Message = updateResult ? "User updated successfully." : "No changes made to the user."
			};
		}

		public async Task<ServiceResponse<bool>> DeleteUserAsync(string id)
		{
			if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
			{
				return new ServiceResponse<bool>()
				{
					Data = false,
					StatusCode = HttpStatusCode.BadRequest,
					Success = false,
					Message = string.IsNullOrEmpty(id) ? "User ID cannot be null or empty." : "Invalid User ID."
				};
			}

			if (!await _userRepository.UserExistsByIdAsync(id))
			{
				return new ServiceResponse<bool>()
				{
					Data = false,
					StatusCode = HttpStatusCode.NotFound,
					Success = false,
					Message = $"User with ID {id} not found."
				};
			}

			var deleteResult = await _userRepository.DeleteUserAsync(id);

			if (deleteResult)
			{
				// Invalidate the cache for the deleted user and the list of all users
				await _cache.RemoveAsync(CachedKeys.UserById(id));
				await _cache.RemoveAsync(CachedKeys.AllUsers);
			}

			return new ServiceResponse<bool>()
			{
				Data = deleteResult,
				StatusCode = deleteResult ? HttpStatusCode.NoContent : HttpStatusCode.InternalServerError,
				Success = deleteResult,
				Message = deleteResult ? "User deleted successfully." : "Error occurred while deleting the user."
			};
		}
	}
}
