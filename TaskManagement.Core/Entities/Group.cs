using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TaskManagement.Core.Entities.BaseEntities;
using TaskManagement.Core.Interfaces.Model;

namespace TaskManagement.Core.Entities
{
	/// <summary>
	/// The Group entity represents a collection of lists, used to organize lists under a common category.
	/// </summary>
	public sealed class Group : BaseEntity, IAudit
	{
		[BsonElement("name")]
		public string Name { get; set; }

		[BsonElement("description")]
		public string Description { get; set; }

		[BsonElement("listIds")]
		public List<string> ListIds { get; set; } = new List<string>();

		[BsonElement("createdBy")]
		public string CreatedBy { get; set; }

		[BsonElement("updatedBy")]
		public string UpdatedBy { get; set; }
	}
}