using System.ComponentModel.DataAnnotations;

namespace Athena.Gate.WebApi.Interop.Requests;

public class CreateServiceAccountRequest
{
    [MaxLength(256)]
    public required string ServiceName { get; set; }
    
    public string[] Scopes { get; set; } = Array.Empty<string>();
}