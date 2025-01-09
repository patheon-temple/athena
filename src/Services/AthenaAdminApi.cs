using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Athena.SDK.Crypto;
using Athena.SDK.Definitions;
using Athena.SDK.Formatters;
using Athena.SDK.Models;
using FluentValidation;

namespace Athena.SDK.Services
{
    public sealed class AthenaAdminApi : IAthenaAdminApi
    {
        private readonly IAthenaApi _athenaApi;
        private readonly IUserRepository _userRepository;
        private readonly IServiceRepository _serviceRepository;

        public AthenaAdminApi( IAthenaApi athenaApi, IServiceRepository serviceRepository, IUserRepository userRepository)
        {
            _athenaApi = athenaApi;
            _serviceRepository = serviceRepository;
            _userRepository = userRepository;
        }

        public async Task<PantheonUser> CreateUserAsync(
            string? deviceId,
            string? username,
            string? password,
            string[] scopes,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(deviceId) && string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException(nameof(deviceId),
                    $"{nameof(deviceId)} and {nameof(username)} cannot be null or empty.");

            if (!string.IsNullOrWhiteSpace(username))
            {
                await IsUsernameAndPasswordValidOrThrowAsync(username, password, cancellationToken);
            }

            scopes = scopes.Where(x => !x.Equals(GlobalDefinitions.Scopes.Superuser)).ToArray();

           return await _userRepository.CreateUserAccountAsync(new PantheonUser
            {
                Scopes = scopes,
                PasswordHash = string.IsNullOrWhiteSpace(password) ? null : Passwords.HashPassword(password),
                Username = username?.ToLower(),
                DeviceId = deviceId
            }, cancellationToken);
        }

        public  Task<PantheonUser?> GetUserAccountByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return _userRepository.GetUserAccountByIdAsync(id, cancellationToken);
        }

        private async Task IsUsernameAndPasswordValidOrThrowAsync(string username, string? password,
            CancellationToken cancellationToken)
        {
            if (await _athenaApi.DoesUsernameExistAsync(username, cancellationToken))
                throw new ValidationException("Username already exists.");

            var passwordValidator = Passwords.CreateValidator();
            var validationResult = await passwordValidator.ValidateAsync(new UserCredentialsValidationParams
            {
                Username = username,
                Password = password
            }, cancellationToken);

            if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);
        }

        public async Task<PantheonService> CreateServiceAccountAsync(string serviceName, string[] requiredScopes,
            CancellationToken cancellationToken = default)
        {
            requiredScopes = requiredScopes.Where(x => !x.Equals(GlobalDefinitions.Scopes.Superuser)).ToArray();
            return await _serviceRepository.CreateServiceAccountAsync(new PantheonService
            {
                Scopes = requiredScopes,
                AuthorizationCode = Encoding.UTF8.GetBytes(GuidFormatters.Stringyfi(Guid.NewGuid())),
                Name = serviceName,
            }, cancellationToken);
        }

        public async Task<PantheonService?> GetServiceAccountByIdAsync(string serviceId,
            CancellationToken cancellationToken = default)
        {
            return await _serviceRepository.GetServiceAccountByIdAsync(serviceId, cancellationToken);
        }

        public Task<bool> ServiceAccountExistsAsync(string serviceId, CancellationToken cancellationToken = default)
        {
            return _serviceRepository.DoesServiceAccountWithIdExistsAsync(serviceId, cancellationToken);
        }
    }
}