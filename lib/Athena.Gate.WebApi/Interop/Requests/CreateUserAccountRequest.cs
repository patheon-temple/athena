namespace Athena.Gate.WebApi.Interop.Requests;

public class CreateUserAccountRequest
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? DeviceId { get; set; }
    public string[] Roles { get; set; } = [];
}