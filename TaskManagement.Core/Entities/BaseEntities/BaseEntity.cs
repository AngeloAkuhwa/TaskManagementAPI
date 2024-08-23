using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TaskManagement.Core.Entities.BaseEntities
{
	public abstract class BaseEntity
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		[BsonElement("createdAt")]
		[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		[BsonElement("updatedAt")]
		[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
		public DateTime? UpdatedAt { get; set; }

		[BsonElement("isDeleted")]
		public bool IsDeleted { get; set; }
	}
}