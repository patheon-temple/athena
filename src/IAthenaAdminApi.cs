using System;
using System.Threading;
using System.Threading.Tasks;
using Athena.SDK.Models;

namespace Athena.SDK
{
    public interface IAthenaAdminApi
    {
        Task<PantheonUser> CreateUserAsync(
            string? deviceId,
            string? username,
            string? password,
            string[] scopes,
            CancellationToken cancellationToken = default);

        Task<PantheonUser?> GetUserAccountByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<PantheonService> CreateServiceAccountAsync(string serviceName, string[] requiredScopes,
            CancellationToken cancellationToken = default);

        Task<PantheonService?> GetServiceAccountByIdAsync(string serviceId,
            CancellationToken cancellationToken = default);

        Task<bool> ServiceAccountExistsAsync(string serviceId, CancellationToken cancellationToken = default);
    }
}