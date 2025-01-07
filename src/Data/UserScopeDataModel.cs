using System.Collections.Generic;

namespace Athena.SDK.Data
{
    public class UserScopeDataModel
    {
        public string Id { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string? Description { get; set; }

        public ICollection<UserAccountDataModel> Accounts { get; set; } = new List<UserAccountDataModel>();
    }
}