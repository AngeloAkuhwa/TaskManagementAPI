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
	public class ListService : IListService
	{
		private readonly IListRepository _listRepository;
		private readonly IDistributedCache _cache;
		private readonly RedisSettings _redisSettings;
		private readonly ILogger<ListService> _logger;

		public ListService(IListRepository listRepository, IDistributedCache cache, IOptions<RedisSettings> redisSettings,
			ILogger<ListService> logger)
		{
			_listRepository = listRepository;
			_cache = cache;
			_redisSettings = redisSettings.Value;
			_logger = logger;
		}

		public async Task<ServiceResponse<List<ResponseListDto>>> GetAllListsAsync()
		{
			LoggingHelper.LogInformation(_logger, nameof(GetAllListsAsync), "GetAllListsAsync request operation started");

			try
			{
				var lists = await _cache.GetOrSetAsync(CachedKeys.AllLists, async () =>
				{
					var listEntities = await _listRepository.GetAllListsAsync();
					return listEntities.Select(l => l.ToDto()).ToList();
				}, _redisSettings);

				LoggingHelper.LogInformation(_logger, nameof(GetAllListsAsync), "Lists retrieved successfully");

				return new ServiceResponse<List<ResponseListDto>>()
				{
					Data = lists != null && lists.Any() ? lists : Enumerable.Empty<ResponseListDto>().ToList(),
					Success = true,
					StatusCode = HttpStatusCode.OK,
					Message = lists != null && lists.Any() ? "Lists retrieved successfully." : "No lists found."
				};
			}
			catch (Exception ex)
			{
				LoggingHelper.LogError(_logger, nameof(GetAllListsAsync), ex.Message);
				return new ServiceResponse<List<ResponseListDto>>()
				{
					Data = Enumerable.Empty<ResponseListDto>().ToList(),
					Success = false,
					StatusCode = HttpStatusCode.InternalServerError,
					Message = "An error occurred while retrieving the lists."
				};
			}
		}

		public async Task<ServiceResponse<ResponseListDto?>> GetListByIdAsync(string id)
		{
			LoggingHelper.LogInformation(_logger, nameof(GetListByIdAsync), "GetListByIdAsync request operation started");

			try
			{
				if (string.IsNullOrWhiteSpace(id) || !ObjectId.TryParse(id, out _))
				{
					var message = !ObjectId.TryParse(id, out _) ? "Invalid ID value" : "List ID cannot be null or empty.";
					LoggingHelper.LogError(_logger, nameof(GetListByIdAsync), message);
					return new ServiceResponse<ResponseListDto?>()
					{
						Data = null,
						StatusCode = HttpStatusCode.BadRequest,
						Success = false,
						Message = message
					};
				}

				var list = await _cache.GetOrSetAsync(CachedKeys.ListById(id), async () =>
				{
					if (!await _listRepository.ListExistsByIdAsync(id))
					{
						return null;
					}

					var listEntity = await _listRepository.GetListByIdAsync(id);
					return listEntity?.ToDto();
				}, _redisSettings);

				if (list == null)
				{
					var notFoundMessage = $"List with ID {id} not found.";
					LoggingHelper.LogError(_logger, nameof(GetListByIdAsync), notFoundMessage);
					return new ServiceResponse<ResponseListDto?>()
					{
						Data = null,
						StatusCode = HttpStatusCode.NotFound,
						Success = false,
						Message = notFoundMessage
					};
				}

				LoggingHelper.LogInformation(_logger, nameof(GetListByIdAsync), $"List with ID '{id}' retrieved successfully");

				return new ServiceResponse<ResponseListDto?>()
				{
					Data = list,
					StatusCode = HttpStatusCode.OK,
					Success = true,
					Message = "List retrieved successfully."
				};
			}
			catch (Exception ex)
			{
				LoggingHelper.LogError(_logger, nameof(GetListByIdAsync), ex.Message);
				return new ServiceResponse<ResponseListDto?>()
				{
					Data = null,
					Success = false,
					StatusCode = HttpStatusCode.InternalServerError,
					Message = "An error occurred while retrieving the list."
				};
			}
		}

		public async Task<ServiceResponse<ResponseListDto?>> CreateListAsync(RequestListDto listDto)
		{
			LoggingHelper.LogInformation(_logger, nameof(CreateListAsync), "CreateListAsync request operation started");

			try
			{
				if (string.IsNullOrWhiteSpace(listDto.Name))
				{
					var message = "List name cannot be null or empty.";
					LoggingHelper.LogError(_logger, nameof(CreateListAsync), message);
					return new ServiceResponse<ResponseListDto?>()
					{
						Data = null,
						StatusCode = HttpStatusCode.BadRequest,
						Success = false,
						Message = message
					};
				}

				if (await _listRepository.ListExistsByNameAsync(listDto.Name))
				{
					var conflictMessage = "A list with the same name already exists.";
					LoggingHelper.LogError(_logger, nameof(CreateListAsync), conflictMessage);
					return new ServiceResponse<ResponseListDto?>()
					{
						Data = null,
						StatusCode = HttpStatusCode.Conflict,
						Success = false,
						Message = conflictMessage
					};
				}

				var list = listDto.ToEntity();
				var createdList = await _listRepository.CreateListAsync(list);

				// Invalidate the cache for the list of all lists
				await _cache.RemoveAsync(CachedKeys.AllLists);

				LoggingHelper.LogInformation(_logger, nameof(CreateListAsync), $"List '{listDto.Name}' created successfully.");

				return new ServiceResponse<ResponseListDto?>()
				{
					Data = createdList.ToDto(),
					StatusCode = HttpStatusCode.Created,
					Success = true,
					Message = "List created successfully."
				};
			}
			catch (Exception ex)
			{
				LoggingHelper.LogError(_logger, nameof(CreateListAsync), ex.Message);
				return new ServiceResponse<ResponseListDto?>()
				{
					Data = null,
					StatusCode = HttpStatusCode.InternalServerError,
					Success = false,
					Message = "An error occurred while creating the list."
				};
			}
		}

		public async Task<ServiceResponse<bool>> UpdateListAsync(RequestListDto listDto)
		{
			LoggingHelper.LogInformation(_logger, nameof(UpdateListAsync), "UpdateListAsync request operation started");

			try
			{
				if (string.IsNullOrWhiteSpace(listDto.Id) || !ObjectId.TryParse(listDto.Id, out _))
				{
					var message = !ObjectId.TryParse(listDto.Id, out _) ? "Invalid List ID." : "List ID cannot be null or empty.";
					LoggingHelper.LogError(_logger, nameof(UpdateListAsync), message);
					return new ServiceResponse<bool>()
					{
						Data = false,
						StatusCode = HttpStatusCode.BadRequest,
						Success = false,
						Message = message
					};
				}

				var updateResult = await _listRepository.UpdateListAsync(listDto.ToEntity());

				if (updateResult)
				{
					// Invalidate the cache for the updated list and the list of all lists
					await _cache.RemoveAsync(CachedKeys.ListById(listDto.Id));
					await _cache.RemoveAsync(CachedKeys.AllLists);

					LoggingHelper.LogInformation(_logger, nameof(UpdateListAsync),
						$"List with ID '{listDto.Id}' updated successfully.");
				}

				return new ServiceResponse<bool>()
				{
					Data = updateResult,
					Success = updateResult,
					StatusCode = updateResult ? HttpStatusCode.NoContent : HttpStatusCode.NotModified,
					Message = updateResult ? "List updated successfully." : "No changes made to the list."
				};
			}
			catch (Exception ex)
			{
				LoggingHelper.LogError(_logger, nameof(UpdateListAsync), ex.Message);
				return new ServiceResponse<bool>()
				{
					Data = false,
					StatusCode = HttpStatusCode.InternalServerError,
					Success = false,
					Message = "An error occurred while updating the list."
				};
			}
		}

		public async Task<ServiceResponse<bool>> DeleteListAsync(string id)
		{
			LoggingHelper.LogInformation(_logger, nameof(DeleteListAsync), "DeleteListAsync request operation started");

			try
			{
				if (string.IsNullOrWhiteSpace(id) || !ObjectId.TryParse(id, out _))
				{
					var message = !ObjectId.TryParse(id, out _) ? "Invalid List ID." : "List ID cannot be null or empty.";
					LoggingHelper.LogError(_logger, nameof(DeleteListAsync), message);
					return new ServiceResponse<bool>()
					{
						Data = false,
						StatusCode = HttpStatusCode.BadRequest,
						Success = false,
						Message = message
					};
				}

				var listExists = await _listRepository.ListExistsByIdAsync(id);

				if (!listExists)
				{
					var notFoundMessage = $"List with ID {id} not found.";
					LoggingHelper.LogError(_logger, nameof(DeleteListAsync), notFoundMessage);
					return new ServiceResponse<bool>()
					{
						Data = false,
						StatusCode = HttpStatusCode.NotFound,
						Success = false,
						Message = notFoundMessage
					};
				}

				var deleteResult = await _listRepository.DeleteListAsync(id);

				if (deleteResult)
				{
					// Invalidate the cache for the deleted list and the list of all lists
					await _cache.RemoveAsync(CachedKeys.ListById(id));
					await _cache.RemoveAsync(CachedKeys.AllLists);

					LoggingHelper.LogInformation(_logger, nameof(DeleteListAsync), $"List with ID '{id}' deleted successfully.");
				}

				return new ServiceResponse<bool>()
				{
					Data = deleteResult,
					StatusCode = deleteResult ? HttpStatusCode.NoContent : HttpStatusCode.InternalServerError,
					Success = deleteResult,
					Message = deleteResult ? "List deleted successfully." : "Error occurred while deleting the list."
				};
			}
			catch (Exception ex)
			{
				LoggingHelper.LogError(_logger, nameof(DeleteListAsync), ex.Message);
				return new ServiceResponse<bool>()
				{
					Data = false,
					StatusCode = HttpStatusCode.InternalServerError,
					Success = false,
					Message = "An error occurred while deleting the list."

				};
			}
		}
	}
}
