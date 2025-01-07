using System.Collections.Generic;

namespace Athena.SDK.Data
{
    public class ServiceScopeDataModel
    {
        public string Id { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string? Description { get; set; }

        public ICollection<ServiceAccountDataModel> Accounts { get; set; } = new List<ServiceAccountDataModel>();
    }
}