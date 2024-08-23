using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TaskManagement.Core.Entities.BaseEntities;
using TaskManagement.Core.Interfaces.Model;

namespace TaskManagement.Core.Entities
{
	/// <summary>
	/// The Task entity represents an individual task in the task management system.
	/// In MongoDB, it would be stored as a document within a collection.
	/// </summary>
	public sealed class ScheduleTask : BaseEntity, IAudit
	{
		[BsonElement("title")]
		public string Title { get; set; }

		[BsonElement("description")]
		public string Description { get; set; }

		[BsonElement("status")]
		public string Status { get; set; }

		[BsonElement("priority")]
		public int Priority { get; set; }

		[BsonElement("listId")]
		[BsonRepresentation(BsonType.ObjectId)]
		public string ListId { get; set; }

		[BsonElement("groupId")]
		[BsonRepresentation(BsonType.ObjectId)]
		public string GroupId { get; set; }

		[BsonElement("assignedUserIds")]
		public List<string> AssignedUserIds { get; set; }

		[BsonElement("createdBy")]
		public string CreatedBy { get; set; }

		[BsonElement("updatedBy")]
		public string UpdatedBy { get; set; }
	}
}