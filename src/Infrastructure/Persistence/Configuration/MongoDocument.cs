using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Infrastructure.Persistence.Configuration;

/// <summary>
/// Represents the base implementation of MongoDb documents
/// </summary>
internal class MongoDocument
{
    /// <summary>
    /// MongoDB document ID
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

    /// <summary>
    /// Value is set during the creation of this element
    /// </summary>
    public DateTime CreatedAt => Id.CreationTime;
}
