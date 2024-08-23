using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using TaskManagement.Core.Commons.DTOs;
using TaskManagement.Core.Commons.Mappers;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces.IRepositories;
using TaskManagement.Infrastructure.Services;
using TaskManagement.Infrastructure.Settings;
using Xunit;

namespace TaskManagement.Tests
{
	public class ListServiceTests
	{
		private readonly Mock<IListRepository> _listRepositoryMock;
		private readonly ListService _listService;

		public ListServiceTests()
		{
			_listRepositoryMock = new Mock<IListRepository>();
			Mock<IDistributedCache> cacheMock = new();
			Mock<IOptions<RedisSettings>> redisSettingsMock = new();
			Mock<ILogger<ListService>> loggerListService = new();

			var redisSettings = new RedisSettings { SlidingExpiration = TimeSpan.FromMinutes(30), AbsoluteExpiration = TimeSpan.FromHours(1) };
			redisSettingsMock.Setup(rs => rs.Value).Returns(redisSettings);

			_listService = new ListService(_listRepositoryMock.Object, cacheMock.Object, redisSettingsMock.Object, loggerListService.Object);
		}

		[Fact]
		public async Task GetAllListsAsync_ShouldReturnLists_WhenListsExist()
		{
			// Arrange
			var lists = new List<ResponseListDto>
			{
				new ResponseListDto(Name: "Name1", Description: "Description1", GroupId: "66c5b64f643f79690b1a7d05", TaskIds: new List<string>(),
					CreatedBy: "CreatedBy", UpdatedBy: "UpdatedBy", Id: "66c5b64f643f79690b1a7d03"),
				new ResponseListDto(Name: "Name2", Description: "Description2", GroupId: "66c5b64f643f79690b1a7d06", TaskIds: new List<string>(),
					CreatedBy: "CreatedBy", UpdatedBy: "UpdatedBy", Id: "66c5b64f643f79690b1a7d04")
			};

			_listRepositoryMock.Setup(repo => repo.GetAllListsAsync())
					.ReturnsAsync(lists.Select(l => l.ToEntity()).ToList());

			// Act
			var result = await _listService.GetAllListsAsync();

			// Assert
			Assert.True(result.Success);
			Assert.Equal(2, result.Data.Count);
			Assert.Equal("Lists retrieved successfully.", result.Message);
		}

		[Fact]
		public async Task GetListByIdAsync_ShouldReturnList_WhenListExists()
		{
			// Arrange
			var list = new ResponseListDto(Name: "Name2", Description: "Description2", GroupId: "66c5b64f643f79690b1a7d06", TaskIds: new List<string>(),
				CreatedBy: "CreatedBy", UpdatedBy: "UpdatedBy", Id: "66c5b64f643f79690b1a7d04");
			_listRepositoryMock.Setup(repo => repo.GetListByIdAsync(It.IsAny<string>()))
					.ReturnsAsync(list.ToEntity());
			_listRepositoryMock.Setup(repo => repo.ListExistsByIdAsync(It.IsAny<string>()))
					.ReturnsAsync(true);

			// Act
			var result = await _listService.GetListByIdAsync(list.Id);

			// Assert
			Assert.True(result.Success);
			Assert.Equal(list.Id, result.Data.Id);
			Assert.Equal("List retrieved successfully.", result.Message);
		}

		[Fact]
		public async Task CreateListAsync_ShouldCreateList_WhenListIsValid()
		{
			// Arrange
			var listDto = new RequestListDto(Name: "Name2", Description: "Description2", GroupId: "66c5b64f643f79690b1a7d06", TaskIds: new List<string>(),
				CreatedBy: "CreatedBy", UpdatedBy: "UpdatedBy", Id: "66c5b64f643f79690b1a7d04");
			var listEntity = listDto.ToEntity();
			var listResponseDto = listEntity.ToDto();
			_listRepositoryMock.Setup(repo => repo.ListExistsByNameAsync(listDto.Name))
					.ReturnsAsync(false);
			_listRepositoryMock.Setup(repo => repo.CreateListAsync(It.IsAny<List>()))
					.ReturnsAsync(listEntity);

			// Act
			var result = await _listService.CreateListAsync(listDto);

			// Assert
			Assert.True(result.Success);
			Assert.Equal("List created successfully.", result.Message);
			Assert.Equal(listDto.Name, result.Data.Name);
		}

		[Fact]
		public async Task UpdateListAsync_ShouldReturnNoContent_WhenUpdateIsSuccessful()
		{
			// Arrange
			var listDto = new RequestListDto(Name: "Name2", Description: "Description2", GroupId: "66c5b64f643f79690b1a7d06", TaskIds: new List<string>(),
				CreatedBy: "CreatedBy", UpdatedBy: "UpdatedBy", Id: "66c5b64f643f79690b1a7d04");
			_listRepositoryMock.Setup(repo => repo.UpdateListAsync(It.IsAny<List>()))
					.ReturnsAsync(true);

			// Act
			var result = await _listService.UpdateListAsync(listDto);

			// Assert
			Assert.True(result.Success);
			Assert.Equal("List updated successfully.", result.Message);
		}

		[Fact]
		public async Task DeleteListAsync_ShouldReturnNoContent_WhenDeleteIsSuccessful()
		{
			// Arrange
			var listId = "66c5b64f643f79690b1a7d07";
			_listRepositoryMock.Setup(repo => repo.ListExistsByIdAsync(listId))
					.ReturnsAsync(true);
			_listRepositoryMock.Setup(repo => repo.DeleteListAsync(listId))
					.ReturnsAsync(true);

			// Act
			var result = await _listService.DeleteListAsync(listId);

			// Assert
			Assert.True(result.Success);
			Assert.Equal("List deleted successfully.", result.Message);
		}
	}
}
