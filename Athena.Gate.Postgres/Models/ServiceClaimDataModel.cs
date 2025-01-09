namespace Athena.Gate.Postgres.Models;

public class ServiceClaimDataModel
{
    public string Id { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? Description { get; set; }

    public ICollection<ServiceAccountDataModel> Accounts { get; set; } = new List<ServiceAccountDataModel>();
}