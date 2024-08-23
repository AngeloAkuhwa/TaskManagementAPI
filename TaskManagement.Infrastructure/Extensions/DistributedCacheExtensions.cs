using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using TaskManagement.Infrastructure.Settings;

namespace TaskManagement.Infrastructure.Extensions
{
	public static class DistributedCacheExtensions
	{
		private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
		{
			PropertyNamingPolicy = null,
			WriteIndented = true,
			AllowTrailingCommas = true,
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
		};

		public static Task SetAsync<T>(this IDistributedCache cache, string key, T value, DistributedCacheEntryOptions options)
		{
			var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value, SerializerOptions));
			return cache.SetAsync(key, bytes, options);
		}

		public static bool TryGetValue<T>(this IDistributedCache cache, string key, out T? value)
		{
			var val = cache.Get(key);
			value = default;

			if (val == null || val.Length == 0)
			{
				return false;
			}

			value = JsonSerializer.Deserialize<T>(val, SerializerOptions);
			return true;
		}

		public static async Task<T?> GetOrSetAsync<T>(this IDistributedCache cache, string key, Func<Task<T>> task, RedisSettings redisSettings)
		{
			if (cache.TryGetValue(key, out T? value) && value is not null)
			{
				return value;
			}

			value = await task();

			if (value is not null)
			{
				var options = new DistributedCacheEntryOptions
				{
					SlidingExpiration = redisSettings.SlidingExpiration,
					AbsoluteExpirationRelativeToNow = redisSettings.AbsoluteExpiration
				};

				await cache.SetAsync(key, value, options);
			}

			return value;
		}
	}
}
