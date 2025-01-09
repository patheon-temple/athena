namespace Athena.Gate.WebApi.Interop.Shared;

internal class ServiceAccount
{
    public required string ServiceName { get; set; }
    public required string Id { get; set; }
    public string? AuthorizationCode { get; set; }
}