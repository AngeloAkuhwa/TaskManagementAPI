using Microsoft.Extensions.Caching.Distributed;
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
	public class UserServiceTests
	{
		private readonly Mock<IUserRepository> _userRepositoryMock;
		private readonly UserService _userService;

		public UserServiceTests()
		{
			_userRepositoryMock = new Mock<IUserRepository>();
			Mock<IDistributedCache> cacheMock = new();
			Mock<IOptions<RedisSettings>> redisSettingsMock = new();

			var redisSettings = new RedisSettings { SlidingExpiration = TimeSpan.FromMinutes(30), AbsoluteExpiration = TimeSpan.FromHours(1) };
			redisSettingsMock.Setup(rs => rs.Value).Returns(redisSettings);

			_userService = new UserService(_userRepositoryMock.Object, cacheMock.Object, redisSettingsMock.Object);
		}

		[Fact]
		public async Task GetAllUsersAsync_ShouldReturnUsers_WhenUsersExist()
		{
			// Arrange
			var users = new List<ResponseUserDto>
			{
				new ResponseUserDto("userName1", "user1", "user1@example.com", "First1", new List<string>(),
					"66c5b64f643f79690b1a7d01"),
				new ResponseUserDto("userName2", "user2", "user2@example.com", "First2", new List<string>(),
					"66c5b64f643f79690b1a7d02")
			};

			_userRepositoryMock.Setup(repo => repo.GetAllUsersAsync())
					.ReturnsAsync(users.Select(u => u.ToEntity()).ToList());

			// Act
			var result = await _userService.GetAllUsersAsync();

			// Assert
			Assert.True(result.Success);
			Assert.Equal(2, result.Data.Count);
			Assert.Equal("Users retrieved successfully.", result.Message);
		}

		[Fact]
		public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
		{
			// Arrange
			var user = new ResponseUserDto("userName", "user1", "user1@example.com", "First1", new List<string>(),  "66c5b64f643f79690b1a7d01");

			_userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(It.IsAny<string>()))
					.ReturnsAsync(user.ToEntity());
			_userRepositoryMock.Setup(repo => repo.UserExistsByIdAsync(It.IsAny<string>()))
					.ReturnsAsync(true);

			// Act
			var result = await _userService.GetUserByIdAsync(user.Id);

			// Assert
			Assert.True(result.Success);
			Assert.Equal(user.Id, result.Data?.Id);
			Assert.Equal("User retrieved successfully.", result.Message);
		}

		[Fact]
		public async Task CreateUserAsync_ShouldCreateUser_WhenUserIsValid()
		{
			// Arrange
			var userDto = new RequestUserDto("user1", "user1@example.com", "First1", "Last1", new List<string>());
			var userEntity = userDto.ToEntity();
			var userResponseDto = userEntity.ToDto();

			_userRepositoryMock.Setup(repo => repo.UserExistsByEmailAsync(userDto.Email))
					.ReturnsAsync(false);
			_userRepositoryMock.Setup(repo => repo.CreateUserAsync(It.IsAny<User>()))
					.ReturnsAsync(userEntity);

			// Act
			var result = await _userService.CreateUserAsync(userDto);

			// Assert
			Assert.True(result.Success);
			Assert.Equal("User created successfully.", result.Message);
			Assert.Equal(userDto.Username, result.Data?.Username);
		}

		[Fact]
		public async Task UpdateUserAsync_ShouldReturnNoContent_WhenUpdateIsSuccessful()
		{
			// Arrange
			var userDto = new RequestUserDto("user1", "user1@example.com", "First1", "Last1", new List<string>(), "66c5b64f643f79690b1a7d01");

			_userRepositoryMock.Setup(repo => repo.UserExistsByIdAsync(It.IsAny<string>()))
					.ReturnsAsync(true);
			_userRepositoryMock.Setup(repo => repo.UpdateUserAsync(It.IsAny<User>()))
				.ReturnsAsync(true);

			// Act
			var result = await _userService.UpdateUserAsync(userDto);

			// Assert
			Assert.True(result.Success);
			Assert.Equal("User updated successfully.", result.Message);
		}

		[Fact]
		public async Task DeleteUserAsync_ShouldReturnNoContent_WhenDeleteIsSuccessful()
		{
			// Arrange
			var userId = "66c5b64f643f79690b1a7d01";
			_userRepositoryMock.Setup(repo => repo.UserExistsByIdAsync(It.IsAny<string>()))
					.ReturnsAsync(true);
			_userRepositoryMock.Setup(repo => repo.DeleteUserAsync(It.IsAny<string>()))
					.ReturnsAsync(true);

			// Act
			var result = await _userService.DeleteUserAsync(userId);

			// Assert
			Assert.True(result.Success);
			Assert.Equal("User deleted successfully.", result.Message);
		}
	}
}
