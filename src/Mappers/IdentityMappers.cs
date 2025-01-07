using System.Linq;
using Athena.SDK.Data;
using Athena.SDK.Models;

namespace Athena.SDK.Mappers
{
    public static class IdentityMappers
    {
        public static PantheonIdentity ToDomain(UserAccountDataModel data) => new PantheonIdentity()
        {
            DeviceId = data.DeviceId,
            Id = data.Id,
            PasswordHash = data.PasswordHash,
            Username = data.Username,
            Scopes = data.Scopes.Select(x => x.Id).ToArray(),
        };
    }
}