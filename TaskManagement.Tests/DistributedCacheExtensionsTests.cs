using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System.Text;
using System.Text.Json;
using TaskManagement.Infrastructure.Extensions;
using TaskManagement.Infrastructure.Settings;
using Xunit;

namespace TaskManagement.Tests
{
	public class DistributedCacheExtensionsTests
	{
		private readonly Mock<IDistributedCache> _cacheMock;
		private readonly RedisSettings _redisSettings;

		public DistributedCacheExtensionsTests()
		{
			_cacheMock = new Mock<IDistributedCache>();
			_redisSettings = new RedisSettings
			{
				SlidingExpiration = TimeSpan.FromMinutes(30),
				AbsoluteExpiration = TimeSpan.FromHours(1)
			};
		}

		[Fact]
		public async Task GetOrSetAsync_ShouldReturnCachedValue_WhenValueExistsInCache()
		{
			// Arrange
			var key = "test_key";
			var expectedValue = "cached_value";
			var serializedValue = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(expectedValue));

			_cacheMock.Setup(c => c.Get(key)).Returns(serializedValue);

			// Act
			var result = await _cacheMock.Object.GetOrSetAsync(key, () => Task.FromResult("new_value"), _redisSettings);

			// Assert
			Assert.Equal(expectedValue, result);
			_cacheMock.Verify(c => c.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), default), Times.Never);
		}

		[Fact]
		public async Task GetOrSetAsync_ShouldCallTaskAndSetCache_WhenValueDoesNotExistInCache()
		{
			// Arrange
			var key = "test_key";
			var expectedValue = "new_value";
			var serializedValue = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(expectedValue));
			_cacheMock.Setup(c => c.Get(key)).Returns((byte[])null);

			// Act
			var result = await _cacheMock.Object.GetOrSetAsync(key, () => Task.FromResult(expectedValue), _redisSettings);

			// Assert
			Assert.Equal(expectedValue, result);
			_cacheMock.Verify(c => c.SetAsync(
					key,
					It.Is<byte[]>(b => b.SequenceEqual(serializedValue)),
					It.IsAny<DistributedCacheEntryOptions>(),
					default), Times.Once);
		}

		[Fact]
		public async Task SetAsync_ShouldStoreSerializedValueInCache()
		{
			// Arrange
			var key = "test_key";
			var value = "some_value";
			var options = new DistributedCacheEntryOptions();
			var serializedValue = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value));

			// Act
			await _cacheMock.Object.SetAsync(key, value, options);

			// Assert
			_cacheMock.Verify(c => c.SetAsync(
					key,
					It.Is<byte[]>(b => b.SequenceEqual(serializedValue)),
					options,
					default), Times.Once);
		}

		[Fact]
		public void TryGetValue_ShouldReturnTrueAndValue_WhenValueExistsInCache()
		{
			// Arrange
			var key = "test_key";
			var expectedValue = "cached_value";
			var serializedValue = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(expectedValue));

			_cacheMock.Setup(c => c.Get(key)).Returns(serializedValue);

			// Act
			var result = _cacheMock.Object.TryGetValue<string>(key, out var actualValue);

			// Assert
			Assert.True(result);
			Assert.Equal(expectedValue, actualValue);
		}

		[Fact]
		public void TryGetValue_ShouldReturnFalse_WhenValueDoesNotExistInCache()
		{
			// Arrange
			var key = "test_key";
			_cacheMock.Setup(c => c.Get(key)).Returns((byte[])null);

			// Act
			var result = _cacheMock.Object.TryGetValue<string>(key, out var actualValue);

			// Assert
			Assert.False(result);
			Assert.Null(actualValue);
		}
	}
}
