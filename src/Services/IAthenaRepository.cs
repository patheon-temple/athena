using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Athena.SDK.Data;

namespace Athena.SDK.Services
{
    public interface IAthenaRepository
    {
        Task<UserAccountDataModel?> GetUserAccountByDeviceIdAsync(string deviceId, CancellationToken cancellationToken);
        Task CreateUserAccountAsync(UserAccountDataModel identity, CancellationToken cancellationToken);
        Task<bool> DoesUsernameExistsAsync(string username, CancellationToken cancellationToken);
        Task<UserAccountDataModel?> GetUserAccountByUsernameAsync(string username, CancellationToken cancellationToken);
        Task<UserAccountDataModel?> GetUserAccountByIdAsync(Guid id, CancellationToken cancellationToken);
        Task UpdateUserAsync(UserAccountDataModel data, CancellationToken cancellationToken = default);
        Task<ServiceAccountDataModel?> GetServiceAccountAsync(Guid serviceId,
            CancellationToken cancellationToken = default);

        Task<ICollection<UserScopeDataModel>> GetUserScopesAsync(string[] scopes, CancellationToken cancellationToken);
        Task<ICollection<ServiceScopeDataModel>> GetServiceAccountScopesAsync(string[] requiredScopes,CancellationToken cancellationToken);
        Task<ServiceAccountDataModel> CreateServiceAccountAsync(ServiceAccountDataModel dataModel,
            CancellationToken cancellationToken);

        Task<bool> HasServiceAccountWithIdAsync(Guid serviceId, CancellationToken cancellationToken);
    }

}