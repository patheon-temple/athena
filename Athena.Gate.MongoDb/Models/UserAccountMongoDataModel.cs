using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Athena.Gate.MongoDb.Models;

public class UserAccountMongoDataModel
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonIgnoreIfDefault] public string[] Scopes { get; set; } = [];

    [BsonIgnoreIfDefault, BsonIgnoreIfNull]
    public string? DeviceId { get; set; }

    [BsonIgnoreIfDefault, BsonIgnoreIfNull]
    public byte[]? PasswordHash { get; set; }

    [BsonIgnoreIfDefault, BsonIgnoreIfNull]
    public string? Username { get; set; }
}