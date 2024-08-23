using MongoDB.Bson;
using MongoDB.Driver;
using TaskManagement.Core.Entities;

namespace TaskManagement.Infrastructure.Data
{
	public class MongoDbContext
	{
		private readonly IMongoDatabase _database;

		public MongoDbContext(IMongoClient mongoClient, string databaseName)
		{
			try
			{
				// Check if the MongoDB server is available
				mongoClient.StartSession();

				_database = mongoClient.GetDatabase(databaseName);
				CreateCollectionsAndIndexes();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Failed to connect to MongoDB: {ex.Message}");
				throw new Exception("Unable to connect to MongoDB. Please check the connection settings.", ex);
			}
		}

		public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
		public IMongoCollection<Group> Groups => _database.GetCollection<Group>("Groups");
		public IMongoCollection<List> Lists => _database.GetCollection<List>("Lists");
		public IMongoCollection<ScheduleTask> ScheduleTasks => _database.GetCollection<ScheduleTask>("ScheduleTasks");

		private void CreateCollectionsAndIndexes()
		{
			// Create collections if they don't exist
			CreateCollectionIfNotExists("Users");
			CreateCollectionIfNotExists("Groups");
			CreateCollectionIfNotExists("Lists");
			CreateCollectionIfNotExists("ScheduleTasks");

			// Ensure indexes if needed (example for Users collection)
			var userIndexes = new CreateIndexModel<User>(
				Builders<User>.IndexKeys.Ascending(u => u.Email),
				new CreateIndexOptions { Unique = true });

			Users.Indexes.CreateOne(userIndexes);

			// Ensure indexes for Groups collection
			var groupIndexes = new CreateIndexModel<Group>(
				Builders<Group>.IndexKeys.Ascending(g => g.Name),
				new CreateIndexOptions { Unique = true });
			Groups.Indexes.CreateOne(groupIndexes);

			// Ensure indexes for Lists collection
			var listIndexes = new CreateIndexModel<List>(
				Builders<List>.IndexKeys.Ascending(l => l.Name).Ascending(l => l.GroupId),
				new CreateIndexOptions { Unique = true });
			Lists.Indexes.CreateOne(listIndexes);

			// Ensure indexes for Tasks collection
			var taskIndexes = new CreateIndexModel<ScheduleTask>(
				Builders<ScheduleTask>.IndexKeys.Ascending(t => t.Title).Ascending(t => t.ListId),
				new CreateIndexOptions { Unique = true });
			ScheduleTasks.Indexes.CreateOne(taskIndexes);
		}

		private void CreateCollectionIfNotExists(string collectionName)
		{
			var filter = new BsonDocument("name", collectionName);
			var collections = _database.ListCollections(new ListCollectionsOptions { Filter = filter }).ToList();
			if (!collections.Any())
			{
				_database.CreateCollection(collectionName);
			}
		}
	}
}