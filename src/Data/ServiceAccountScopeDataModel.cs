namespace Athena.SDK.Data
{
    public class ServiceAccountScopeDataModel
    {
        public ServiceAccountDataModel Account { get; set; } = null!;
        public ServiceScopeDataModel Scope { get; set; } = null!;
    }
}