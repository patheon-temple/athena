using Athena.Gate.Postgres.Models;
using Athena.SDK.Formatters;
using Athena.SDK.Models;

namespace Athena.Gate.Postgres.Mappers;

public static class ServiceMappers
{
    public static PantheonService ToDomain(ServiceAccountDataModel data) => new()
    {
        Id = GuidFormatters.Stringyfi(data.Id),
        Name = data.Name,
        AuthorizationCode = data.AuthorizationCode,
        Scopes = data.Scopes.Select(x => x.Id).ToArray(),
    };

    public static ServiceAccountDataModel ToDataModel(PantheonService service)=> new()
    {
        Id =  string.IsNullOrWhiteSpace(service.Id) ? Guid.NewGuid() : Guid.Parse(service.Id),
        Name = service.Name,
        AuthorizationCode = service.AuthorizationCode,
        Scopes = service.Scopes.Select(x => new ServiceScopeDataModel
        {
            Id = x
        }).ToArray(),
    };
}