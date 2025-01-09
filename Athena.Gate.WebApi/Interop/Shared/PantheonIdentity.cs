namespace Athena.Gate.WebApi.Interop.Shared;

public class PantheonIdentity(global::Athena.SDK.Models.PantheonUser pantheonUser)
{
    public string Id { get; init; } = pantheonUser.Id;
    public string? DeviceId { get; init; } = pantheonUser.DeviceId;
    public string? Username { get; init; } = pantheonUser.Username;
}