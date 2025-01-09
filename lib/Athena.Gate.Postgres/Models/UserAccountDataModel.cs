namespace Athena.Gate.Postgres.Models;

public class UserAccountDataModel 
{
    public Guid Id { get; set; }
    public string? DeviceId { get; set; }
    public string? Username { get; set; }
    public byte[]? PasswordHash { get; set; }
    public ICollection<UserRoleDataModel> Roles { get; set; } = new List<UserRoleDataModel>();
}