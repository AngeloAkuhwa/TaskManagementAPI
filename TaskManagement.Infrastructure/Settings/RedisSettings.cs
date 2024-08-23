namespace TaskManagement.Infrastructure.Settings
{
	public class RedisSettings
	{
		public string ConnectionString { get; set; }
		public TimeSpan SlidingExpiration { get; set; }
		public TimeSpan AbsoluteExpiration { get; set; }
	}
}
