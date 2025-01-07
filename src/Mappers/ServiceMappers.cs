using System.Linq;
using Athena.SDK.Data;
using Athena.SDK.Models;

namespace Athena.SDK.Mappers
{
    public static class ServiceMappers
    {
        public static PantheonService ToDomain(ServiceAccountDataModel data) => new PantheonService()
        {
            Id = data.Id,
            Name = data.Name,
            AuthorizationCode = data.AuthorizationCode,
            Scopes = data.Scopes.Select(x => x.Id).ToArray(),
        };
    }
}