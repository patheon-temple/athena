using Athena.Gate.Postgres.Models;
using Athena.SDK.Formatters;
using Athena.SDK.Models;

namespace Athena.Gate.Postgres.Mappers;

public static class IdentityMappers
{
    public static PantheonUser ToDomain(UserAccountDataModel data) => new()
    {
        DeviceId = data.DeviceId,
        Id = GuidFormatters.Stringyfi(data.Id),
        PasswordHash = data.PasswordHash,
        Username = data.Username,
        Roles = data.Roles.Select(x => x.Id).ToArray(),
    };

    public static UserAccountDataModel ToDataModel(PantheonUser user)=>new ()
    {
        DeviceId = user.DeviceId,
        Id = string.IsNullOrWhiteSpace(user.Id) ? Guid.NewGuid() : Guid.Parse(user.Id),
        PasswordHash = user.PasswordHash,
        Username = user.Username,
        Roles = user.Roles.Select(x => new UserRoleDataModel
        {
            Id = x
        }).ToList(),
    };
}