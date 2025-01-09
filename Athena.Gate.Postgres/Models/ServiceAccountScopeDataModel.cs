namespace Athena.Gate.Postgres.Models;

    public class ServiceAccountScopeDataModel
    {
        public ServiceAccountDataModel Account { get; set; } = null!;
        public ServiceScopeDataModel Scope { get; set; } = null!;
    }
