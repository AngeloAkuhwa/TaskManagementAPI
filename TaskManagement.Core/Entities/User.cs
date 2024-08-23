using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TaskManagement.Core.Entities.BaseEntities;

namespace TaskManagement.Core.Entities
{
	/// <summary>
	/// The User entity represents a user in the system, who can be assigned tasks.
	/// </summary>
	public sealed class User : BaseEntity
	{
		[BsonElement("username")]
		public string Username { get; set; }

		[BsonElement("email")]
		public string Email { get; set; }

		[BsonElement("firstName")]
		public string FirstName { get; set; }

		[BsonElement("lastName")]
		public string LastName { get; set; }

		[BsonElement("passwordHash")]
		public string PasswordHash { get; set; }

		[BsonElement("assignedTaskIds")]
		public List<string> AssignedTaskIds { get; set; } = new List<string>();
	}
}