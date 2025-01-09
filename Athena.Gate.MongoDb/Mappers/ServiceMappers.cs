using Athena.Gate.MongoDb.Models;
using Athena.SDK.Models;

namespace Athena.Gate.MongoDb.Mappers;

public static class ServiceMappers
{
    public static PantheonService ToDomain(ServiceAccountMongoDataModel dataModel)
    {
        return new PantheonService
        {
            Id = dataModel.Id,
            Name = dataModel.Name,
            Claims = dataModel.Claims,
            AuthorizationCode = dataModel.AuthorizationCode
        };
    }
    
    public static ServiceAccountMongoDataModel ToDataModel(PantheonService dataModel)
    {
        return new ServiceAccountMongoDataModel
        {
            Id = dataModel.Id,
            Name = dataModel.Name,
            Claims = dataModel.Claims,
            AuthorizationCode = dataModel.AuthorizationCode
        };
    }
}