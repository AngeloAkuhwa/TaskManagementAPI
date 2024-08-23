namespace TaskManagement.Infrastructure.Extensions
{
	public static class CachedKeys
	{
		public const string AllGroups = "all_groups";
		public const string AllLists = "all_lists";
		public const string AllTasks = "all_tasks";
		public const string AllUsers = "all_users";

		public static string GroupById(string id) => $"group_{id}";
		public static string ListById(string id) => $"list_{id}";
		public static string TaskById(string id) => $"task_{id}";
		public static string UserById(string id) => $"user_{id}";
	}
}
