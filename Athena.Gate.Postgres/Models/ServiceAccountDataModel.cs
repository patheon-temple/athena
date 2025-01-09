namespace Athena.Gate.Postgres.Models;

public class ServiceAccountDataModel 
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public byte[] AuthorizationCode { get; set; } = Array.Empty<byte>();
    public ICollection<ServiceScopeDataModel> Scopes { get; set; } = new List<ServiceScopeDataModel>();
}