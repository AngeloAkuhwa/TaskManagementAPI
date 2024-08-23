using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Net;
using TaskManagement.Core.Commons.DTOs;
using TaskManagement.Core.Commons.Mappers;
using TaskManagement.Core.Commons.Responses;
using TaskManagement.Core.Interfaces.IRepositories;
using TaskManagement.Core.Interfaces.IServices;
using TaskManagement.Infrastructure.Extensions;
using TaskManagement.Infrastructure.Helpers;
using TaskManagement.Infrastructure.Settings;

namespace TaskManagement.Infrastructure.Services
{
	public class GroupService : IGroupService
	{
		private readonly IGroupRepository _groupRepository;
		private readonly IDistributedCache _cache;
		private readonly RedisSettings _redisSettings;
		private readonly ILogger<GroupService> _logger;

		public GroupService(IGroupRepository groupRepository, IDistributedCache cache, IOptions<RedisSettings> redisSettings, ILogger<GroupService> logger)
		{
			_groupRepository = groupRepository;
			_cache = cache;
			_redisSettings = redisSettings.Value;
			_logger = logger;
		}

		public async Task<ServiceResponse<List<GroupResponseDto>>> GetAllGroupsAsync()
		{
			LoggingHelper.LogInformation(_logger, nameof(GetAllGroupsAsync), "GetAllGroups request operation started");

			try
			{
				var groups = await _cache.GetOrSetAsync(CachedKeys.AllGroups, async () =>
				{
					var groupEntities = await _groupRepository.GetAllGroupsAsync();
					return groupEntities.Select(g => g.ToDto()).ToList();
				}, _redisSettings);

				return new ServiceResponse<List<GroupResponseDto>>()
				{
					Data = groups != null && groups.Any() ? groups : Enumerable.Empty<GroupResponseDto>().ToList(),
					Success = true,
					StatusCode = HttpStatusCode.OK,
					Message = groups != null && groups.Any() ? "Groups retrieved successfully." : "No groups found."
				};
			}
			catch (Exception ex)
			{
				LoggingHelper.LogError(_logger, nameof(GetAllGroupsAsync), ex.Message);
				return new ServiceResponse<List<GroupResponseDto>>()
				{
					Data = new List<GroupResponseDto>(),
					Success = false,
					StatusCode = HttpStatusCode.InternalServerError,
					Message = "An error occurred while retrieving groups."
				};
			}
		}


		public async Task<ServiceResponse<GroupResponseDto?>> GetGroupByIdAsync( string id)
		{
			LoggingHelper.LogInformation(_logger, nameof(GetGroupByIdAsync), "GetGroupByIdAsync request operation started");

			try
			{
				if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
				{
					var message = !ObjectId.TryParse(id, out _) ? "Invalid ID value" : "Group ID cannot be null or empty.";
					LoggingHelper.LogError(_logger, nameof(GetGroupByIdAsync), message);

					return new ServiceResponse<GroupResponseDto?>()
					{
						Data = null,
						StatusCode = HttpStatusCode.BadRequest,
						Success = false,
						Message = message
					};
				}

				var group = await _cache.GetOrSetAsync(CachedKeys.GroupById(id), async () =>
				{
					if (!await _groupRepository.GroupExistsByIdAsync(id))
					{
						return null;
					}

					var groupEntity = await _groupRepository.GetGroupByIdAsync(id);
					return groupEntity?.ToDto();
				}, _redisSettings);

				if (group == null)
				{
					var notFoundMessage = $"Group with ID {id} not found.";
					LoggingHelper.LogError(_logger, nameof(GetGroupByIdAsync), notFoundMessage);

					return new ServiceResponse<GroupResponseDto?>()
					{
						Data = null,
						StatusCode = HttpStatusCode.NotFound,
						Success = false,
						Message = notFoundMessage
					};
				}

				LoggingHelper.LogInformation(_logger, nameof(GetGroupByIdAsync), $"Group with ID {id} retrieved successfully.");

				return new ServiceResponse<GroupResponseDto?>()
				{
					Data = group,
					StatusCode = HttpStatusCode.OK,
					Success = true,
					Message = "Group retrieved successfully."
				};
			}
			catch (Exception ex)
			{
				LoggingHelper.LogError(_logger, nameof(GetGroupByIdAsync), ex.Message);
				return new ServiceResponse<GroupResponseDto?>()
				{
					Data = null,
					StatusCode = HttpStatusCode.InternalServerError,
					Success = false,
					Message = "An error occurred while retrieving the group."
				};
			}
		}

		public async Task<ServiceResponse<GroupResponseDto?>> CreateGroupAsync( GroupRequestDto groupDto)
		{
			LoggingHelper.LogInformation(_logger, nameof(CreateGroupAsync), "CreateGroupAsync request operation started");

			try
			{
				if (string.IsNullOrWhiteSpace(groupDto.Name))
				{
					var message = "Group name cannot be null or empty.";
					LoggingHelper.LogError(_logger, nameof(CreateGroupAsync), message);
					return new ServiceResponse<GroupResponseDto?>()
					{
						Data = null,
						StatusCode = HttpStatusCode.BadRequest,
						Success = false,
						Message = message
					};
				}

				var existingGroup = await _groupRepository.GroupExistsByNameAsync(groupDto.Name);

				if (existingGroup)
				{
					var conflictMessage = "A group with the same name already exists.";
					LoggingHelper.LogError(_logger, nameof(CreateGroupAsync), conflictMessage);
					return new ServiceResponse<GroupResponseDto?>()
					{
						Data = null,
						StatusCode = HttpStatusCode.Conflict,
						Success = false,
						Message = conflictMessage
					};
				}

				var group = groupDto.ToEntity();
				var createdGroup = await _groupRepository.CreateGroupAsync(group);

				LoggingHelper.LogInformation(_logger, nameof(CreateGroupAsync), $"Group '{groupDto.Name}' created successfully.");

				return new ServiceResponse<GroupResponseDto?>()
				{
					Data = createdGroup.ToDto(),
					StatusCode = HttpStatusCode.Created,
					Success = true,
					Message = "Group created successfully."
				};
			}
			catch (Exception ex)
			{
				LoggingHelper.LogError(_logger, nameof(CreateGroupAsync), ex.Message);
				return new ServiceResponse<GroupResponseDto?>()
				{
					Data = null,
					StatusCode = HttpStatusCode.InternalServerError,
					Success = false,
					Message = "An error occurred while creating the group."
				};
			}
		}

		public async Task<ServiceResponse<bool>> UpdateGroupAsync(GroupRequestDto groupDto)
		{
			LoggingHelper.LogInformation(_logger, nameof(UpdateGroupAsync), "UpdateGroupAsync request operation started");

			try
			{
				if (string.IsNullOrWhiteSpace(groupDto.Id) || !ObjectId.TryParse(groupDto.Id, out _))
				{
					var message = !ObjectId.TryParse(groupDto.Id, out _) ? "Invalid Group ID." : "Group ID cannot be null or empty.";
					LoggingHelper.LogError(_logger, nameof(UpdateGroupAsync), message);
					return new ServiceResponse<bool>()
					{
						Data = false,
						StatusCode = HttpStatusCode.BadRequest,
						Success = false,
						Message = message
					};
				}

				var updateResult = await _groupRepository.UpdateGroupAsync(groupDto.ToEntity());

				if (updateResult)
				{
					await _cache.RemoveAsync(CachedKeys.GroupById(groupDto.Id));
					await _cache.RemoveAsync(CachedKeys.AllGroups);

					LoggingHelper.LogInformation(_logger, nameof(UpdateGroupAsync), $"Group with ID '{groupDto.Id}' updated successfully.");
				}

				return new ServiceResponse<bool>()
				{
					Data = updateResult,
					Success = updateResult,
					StatusCode = updateResult ? HttpStatusCode.NoContent : HttpStatusCode.NotModified,
					Message = updateResult ? "Group updated successfully." : "No changes made to the group."
				};
			}
			catch (Exception ex)
			{
				LoggingHelper.LogError(_logger, nameof(UpdateGroupAsync), ex.Message);
				return new ServiceResponse<bool>()
				{
					Data = false,
					StatusCode = HttpStatusCode.InternalServerError,
					Success = false,
					Message = "An error occurred while updating the group."
				};
			}
		}

		public async Task<ServiceResponse<bool>> DeleteGroupAsync(string id)
		{
			LoggingHelper.LogInformation(_logger, nameof(DeleteGroupAsync), "DeleteGroupAsync request operation started");

			try
			{
				if (string.IsNullOrWhiteSpace(id) || !ObjectId.TryParse(id, out _))
				{
					var message = !ObjectId.TryParse(id, out _) ? "Invalid Group ID." : "Group ID cannot be null or empty.";
					LoggingHelper.LogError(_logger, nameof(DeleteGroupAsync), message);
					return new ServiceResponse<bool>()
					{
						Data = false,
						StatusCode = HttpStatusCode.BadRequest,
						Success = false,
						Message = message
					};
				}

				var groupExists = await _groupRepository.GroupExistsByIdAsync(id);

				if (!groupExists)
				{
					var notFoundMessage = $"Group with ID {id} not found.";
					LoggingHelper.LogError(_logger, nameof(DeleteGroupAsync), notFoundMessage);
					return new ServiceResponse<bool>()
					{
						Data = false,
						StatusCode = HttpStatusCode.NotFound,
						Success = false,
						Message = notFoundMessage
					};
				}

				var deleteResult = await _groupRepository.DeleteGroupAsync(id);

				if (deleteResult)
				{
					await _cache.RemoveAsync(CachedKeys.GroupById(id));
					await _cache.RemoveAsync(CachedKeys.AllGroups);

					LoggingHelper.LogInformation(_logger, nameof(DeleteGroupAsync), $"Group with ID '{id}' deleted successfully.");
				}

				return new ServiceResponse<bool>()
				{
					Data = deleteResult,
					StatusCode = deleteResult ? HttpStatusCode.NoContent : HttpStatusCode.InternalServerError,
					Success = deleteResult,
					Message = deleteResult ? "Group deleted successfully." : "Error occurred while deleting the group."
				};
			}
			catch (Exception ex)
			{
				LoggingHelper.LogError(_logger, nameof(DeleteGroupAsync), ex.Message);
				return new ServiceResponse<bool>()
				{
					Data = false,
					StatusCode = HttpStatusCode.InternalServerError,
					Success = false,
					Message = "An error occurred while deleting the group."
				};
			}
		}
	}
}