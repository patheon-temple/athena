namespace Athena.SDK.Data
{
    public class UserAccountScopeDataModel
    {
        public UserAccountDataModel Account { get; set; } = null!;
        public UserScopeDataModel Scope { get; set; } = null!;
    }
}