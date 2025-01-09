using Athena.Gate.MongoDb.Models;
using Athena.SDK.Models;

namespace Athena.Gate.MongoDb.Mappers;

public static class UserMappers
{
    public static PantheonUser ToDomain(UserAccountMongoDataModel dataModel)
    {
        return new PantheonUser
        {
            Id = dataModel.Id,
            Scopes = dataModel.Scopes,
            DeviceId = dataModel.DeviceId,
            PasswordHash = dataModel.PasswordHash,
            Username = dataModel.Username
        };
    }
    
    public static UserAccountMongoDataModel ToDataModel(PantheonUser dataModel)
    {
        return new UserAccountMongoDataModel
        {
            Id = dataModel.Id,
            Scopes = dataModel.Scopes,
            Username = dataModel.Username,
            DeviceId = dataModel.DeviceId,
            PasswordHash = dataModel.PasswordHash,
        };
    }
}