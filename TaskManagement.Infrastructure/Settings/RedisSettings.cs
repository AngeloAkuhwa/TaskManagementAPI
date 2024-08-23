namespace TaskManagement.Infrastructure.Settings
{
	public class RedisSettings
	{
		public string ConnectionString { get; set; }
		public string Password { get; set; }
		public int SyncTimeOut { get; set; }
		public TimeSpan SlidingExpiration { get; set; }
		public TimeSpan AbsoluteExpiration { get; set; }
	}
}
