using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Athena.SDK.Models;

namespace Athena.SDK.Services
{
    public interface IUserRepository
    {
        Task<PantheonUser?> GetUserAccountByDeviceIdAsync(string deviceId, CancellationToken cancellationToken);
        Task<PantheonUser> CreateUserAccountAsync(PantheonUser user, CancellationToken cancellationToken);
        Task<bool> DoesUsernameExistsAsync(string username, CancellationToken cancellationToken);
        Task<PantheonUser?> GetUserAccountByUsernameAsync(string username, CancellationToken cancellationToken);
        Task<PantheonUser?> GetUserAccountByIdAsync(string id, CancellationToken cancellationToken);
        Task UpdateUserAsync(PantheonUser user, CancellationToken cancellationToken = default);
    }
}