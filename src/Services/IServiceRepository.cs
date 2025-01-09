using System;
using System.Threading;
using System.Threading.Tasks;
using Athena.SDK.Models;

namespace Athena.SDK.Services
{
    public interface IServiceRepository
    {     Task<PantheonService?> GetServiceAccountByIdAsync(string serviceId,
            CancellationToken cancellationToken = default);

        Task<PantheonService> CreateServiceAccountAsync(PantheonService service,
            CancellationToken cancellationToken);

        Task<bool> DoesServiceAccountWithIdExistsAsync(string serviceId, CancellationToken cancellationToken);
        
    }

}