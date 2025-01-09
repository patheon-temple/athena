using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Athena.Gate.MongoDb.Models;

public class ServiceAccountMongoDataModel
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonIgnoreIfDefault] public byte[] AuthorizationCode { get; set; } = [];

    [BsonIgnoreIfDefault] public string[] Roles { get; set; } = [];

    [BsonIgnoreIfDefault] public string Name { get; set; } = string.Empty;
}