using System;
using System.Collections.Generic;

namespace Athena.SDK.Data
{
    public class ServiceAccountDataModel 
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public byte[] AuthorizationCode { get; set; } = Array.Empty<byte>();
        public ICollection<ServiceScopeDataModel> Scopes { get; set; } = new List<ServiceScopeDataModel>();
    }
}
