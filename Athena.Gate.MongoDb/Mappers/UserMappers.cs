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
            Claims = dataModel.Claims,
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
            Claims = dataModel.Claims,
            Username = dataModel.Username,
            DeviceId = dataModel.DeviceId,
            PasswordHash = dataModel.PasswordHash,
        };
    }
}