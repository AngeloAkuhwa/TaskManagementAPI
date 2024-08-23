using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TaskManagement.Core.Entities.BaseEntities;
using TaskManagement.Core.Interfaces.Model;

namespace TaskManagement.Core.Entities
{
	/// <summary>
	/// The List entity represents a collection of tasks, typically used to categorize tasks within a group.
	/// </summary>
	public sealed class List : BaseEntity, IAudit
	{
		[BsonElement("name")]
		public string Name { get; set; }

		[BsonElement("description")]
		public string Description { get; set; }

		[BsonElement("groupId")]
		[BsonRepresentation(BsonType.ObjectId)]
		public string GroupId { get; set; }

		[BsonElement("taskIds")]
		public List<string> TaskIds { get; set; } = new List<string>();

		[BsonElement("createdBy")]
		public string CreatedBy { get; set; }

		[BsonElement("updatedBy")]
		public string UpdatedBy { get; set; }
	}
}