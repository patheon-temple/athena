namespace Athena.Gate.Postgres.Models;

    public class ServiceAccountScopeDataModel
    {
        public ServiceAccountDataModel Account { get; set; } = null!;
        public ServiceClaimDataModel Claim { get; set; } = null!;
    }
