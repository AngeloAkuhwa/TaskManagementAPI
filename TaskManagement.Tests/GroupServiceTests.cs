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
	public class GroupServiceTests
	{
		private readonly Mock<IGroupRepository> _groupRepositoryMock;
		private readonly GroupService _groupService;

		public GroupServiceTests()
		{
			_groupRepositoryMock = new Mock<IGroupRepository>();
			Mock<IDistributedCache> cacheMock = new();
			Mock<IOptions<RedisSettings>> redisSettingsMock = new();
			Mock<ILogger<GroupService>> loggerGroupService = new();

			var redisSettings = new RedisSettings { SlidingExpiration = TimeSpan.FromMinutes(30), AbsoluteExpiration = TimeSpan.FromHours(1) };
			redisSettingsMock.Setup(rs => rs.Value).Returns(redisSettings);

			_groupService = new GroupService(_groupRepositoryMock.Object, cacheMock.Object, redisSettingsMock.Object, loggerGroupService.Object);
		}

		[Fact]
		public async Task GetAllGroupsAsync_ShouldReturnGroups_WhenGroupsExist()
		{
			// Arrange
			var groups = new List<GroupResponseDto>
			{
				new GroupResponseDto("1", "Group1", "Description1", new List<string>(), "User1", "User1"),
				new GroupResponseDto("2", "Group2", "Description2", new List<string>(), "User2", "User2")
			};

			_groupRepositoryMock.Setup(repo => repo.GetAllGroupsAsync())
					.ReturnsAsync(groups.Select(g => g.ToEntity()).ToList());

			// Act
			var result = await _groupService.GetAllGroupsAsync();

			// Assert
			Assert.True(result.Success);
			Assert.Equal(2, result.Data.Count);
			Assert.Equal("Groups retrieved successfully.", result.Message);
		}

		[Fact]
		public async Task GetGroupByIdAsync_ShouldReturnGroup_WhenGroupExists()
		{
			// Arrange
			var groupId = "66c5b64f643f79690b1a7d03";
			var groupDto = new GroupResponseDto(groupId, "Group1","Descriptions", new List<string>(), "User1", "User1");
			_groupRepositoryMock.Setup(repo => repo.GetGroupByIdAsync(groupId))
					.ReturnsAsync(groupDto.ToEntity());

			_groupRepositoryMock.Setup(repo => repo.GroupExistsByIdAsync(groupId))
					.ReturnsAsync(true);

			// Act
			var result = await _groupService.GetGroupByIdAsync(groupId);

			// Assert
			Assert.True(result.Success);
			Assert.Equal(groupId, result.Data?.Id);
			Assert.Equal("Group retrieved successfully.", result.Message);
		}

		[Fact]
		public async Task CreateGroupAsync_ShouldCreateGroup_WhenGroupIsValid()
		{
			// Arrange
			var groupDto = new GroupRequestDto("NewGroup", "Description", new List<string>(), "User1", "User1");
			var groupEntity = groupDto.ToEntity();
				 groupEntity.Id = "66c5b64f643f79690b1a7d03";

			_groupRepositoryMock.Setup(repo => repo.GroupExistsByNameAsync(groupDto.Name))
					.ReturnsAsync(false);

			_groupRepositoryMock.Setup(repo => repo.CreateGroupAsync(It.IsAny<Group>()))
				.ReturnsAsync(groupEntity);

			// Act
			var result = await _groupService.CreateGroupAsync(groupDto);

			// Assert
			Assert.True(result.Success);
			Assert.Equal("Group created successfully.", result.Message);
			Assert.Equal(groupDto.Name, result.Data?.Name);
		}

		[Fact]
		public async Task UpdateGroupAsync_ShouldReturnNoContent_WhenUpdateIsSuccessful()
		{
			// Arrange
			var groupDto = new GroupRequestDto("UpdatedGroup", "UpdatedDescription", new List<string>(), "User1", "User1", "1");
				var newGroupDto = groupDto with { Id = "66c5b64f643f79690b1a7d03" };
			_groupRepositoryMock.Setup(repo => repo.UpdateGroupAsync(It.IsAny<Group>()))
					.ReturnsAsync(true);

			// Act
			var result = await _groupService.UpdateGroupAsync(newGroupDto);

			// Assert
			Assert.True(result.Success);
			Assert.Equal("Group updated successfully.", result.Message);
		}

		[Fact]
		public async Task DeleteGroupAsync_ShouldReturnNoContent_WhenDeleteIsSuccessful()
		{
			// Arrange
			var groupId = "66c5b64f643f79690b1a7d03";

			_groupRepositoryMock.Setup(repo => repo.GroupExistsByIdAsync(It.IsAny<string>()))
					.ReturnsAsync(true);

			_groupRepositoryMock.Setup(repo => repo.DeleteGroupAsync(It.IsAny<string>()))
					.ReturnsAsync(true);

			// Act
			var result = await _groupService.DeleteGroupAsync(groupId);

			// Assert
			Assert.True(result.Success);
			Assert.Equal("Group deleted successfully.", result.Message);
		}
	}
}
