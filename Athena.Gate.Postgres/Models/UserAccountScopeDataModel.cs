namespace Athena.Gate.Postgres.Models;

public class UserAccountScopeDataModel
{
    public UserAccountDataModel Account { get; set; } = null!;
    public UserClaimDataModel Claim { get; set; } = null!;
}