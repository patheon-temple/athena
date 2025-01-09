namespace Athena.Gate.Postgres.Models;

public class UserAccountScopeDataModel
{
    public UserAccountDataModel Account { get; set; } = null!;
    public UserRoleDataModel Role { get; set; } = null!;
}