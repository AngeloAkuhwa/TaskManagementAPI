using MongoDB.Driver;
using TaskManagement.Core.Commons.Enums;
using TaskManagement.Core.Entities;

namespace TaskManagement.Infrastructure.Data
{
    public class DatabaseSeeder
	{
		private readonly MongoDbContext _context;

		public DatabaseSeeder(MongoDbContext context)
		{
			_context = context;
		}

		public async Task SeedAsync()
		{
			await SeedUsersAsync();
			await SeedGroupsAsync();
			await SeedListsAsync();
			await SeedTasksAsync();
		}

		private async Task SeedUsersAsync()
		{
			if (await _context.Users.CountDocumentsAsync(_ => true) == 0)
			{
				var users = new List<User>
								{
										new User
										{
												Username = "john_doe",
												Email = "john@gmail.com",
												FirstName = "John",
												LastName = "Doe",
												PasswordHash = BCrypt.Net.BCrypt.HashPassword("hashedpassword1", 12),
												AssignedTaskIds = new List<string>(),
												CreatedAt = DateTime.UtcNow
										},
										new User
										{
												Username = "jane_doe",
												Email = "jane@gmail.com",
												FirstName = "Jane",
												LastName = "Doe",
												PasswordHash = BCrypt.Net.BCrypt.HashPassword("hashedpassword2", 12),
												AssignedTaskIds = new List<string>(),
												CreatedAt = DateTime.UtcNow
										}
								};

				await _context.Users.InsertManyAsync(users);
			}
		}

		private async Task SeedGroupsAsync()
		{
			if (await _context.Groups.CountDocumentsAsync(_ => true) == 0)
			{
				var groups = new List<Group>
								{
										new Group
										{
												Name = "Work",
												Description = "Work-related tasks",
												ListIds = new List<string>(),
												CreatedAt = DateTime.UtcNow	,
												CreatedBy = "john@gmail.com",
												UpdatedBy = "john@gmail.com"
										},
										new Group
										{
												Name = "Personal",
												Description = "Personal tasks",
												ListIds = new List<string>(),
												CreatedAt = DateTime.UtcNow,
												CreatedBy = "john@gmail.com",
												UpdatedBy = "john@gmail.com"
										}
								};

				await _context.Groups.InsertManyAsync(groups);
			}
		}

		private async Task SeedListsAsync()
		{
			if (await _context.Lists.CountDocumentsAsync(_ => true) == 0)
			{
				var workGroup = await _context.Groups.Find(g => g.Name == "Work").FirstOrDefaultAsync();
				var personalGroup = await _context.Groups.Find(g => g.Name == "Personal").FirstOrDefaultAsync();

				var lists = new List<List>
								{
										new List
										{
												Name = "Project A",
												Description = "Tasks for Project A",
												GroupId = workGroup.Id,
												TaskIds = new List<string>(),
												CreatedAt = DateTime.UtcNow,
												CreatedBy = "john@gmail.com",
												UpdatedBy = "john@gmail.com"
										},
										new List
										{
												Name = "Grocery List",
												Description = "Items to buy",
												GroupId = personalGroup.Id,
												TaskIds = new List<string>(),
												CreatedAt = DateTime.UtcNow,
												CreatedBy = "john@gmail.com",
												UpdatedBy = "john@gmail.com"
										}
								};

				await _context.Lists.InsertManyAsync(lists);
			}
		}

		private async Task SeedTasksAsync()
		{
			if (await _context.ScheduleTasks.CountDocumentsAsync(_ => true) == 0)
			{
				var projectAList = await _context.Lists.Find(l => l.Name == "Project A").FirstOrDefaultAsync();
				var groceryList = await _context.Lists.Find(l => l.Name == "Grocery List").FirstOrDefaultAsync();

				var tasks = new List<ScheduleTask>
								{
										new ScheduleTask
										{
												Title = "Complete the report",
												Description = "Finish the quarterly report",
												Status = "Pending",
												Priority = (int)TaskPriority.High,
												ListId = projectAList.Id,
												GroupId = projectAList.GroupId,
												AssignedUserIds = new List<string>(),
												CreatedAt = DateTime.UtcNow,
												CreatedBy = "john@gmail.com",
												UpdatedBy = "john@gmail.com"
										},
										new ScheduleTask
										{
												Title = "Buy milk",
												Description = "Buy 2 liters of milk",
												Status = "Completed",
												Priority = (int)TaskPriority.Medium,
												ListId = groceryList.Id,
												GroupId = groceryList.GroupId,
												AssignedUserIds = new List<string>(),
												CreatedAt = DateTime.UtcNow,
												CreatedBy = "john@gmail.com",
												UpdatedBy = "john@gmail.com"
										}
								};

				await _context.ScheduleTasks.InsertManyAsync(tasks);
			}
		}
	}
}
