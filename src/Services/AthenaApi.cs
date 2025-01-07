using System;
using System.Threading;
using System.Threading.Tasks;
using Athena.SDK.Data;
using Athena.SDK.Mappers;
using Athena.SDK.Models;

namespace Athena.SDK.Services
{
    internal sealed class AthenaApi : IAthenaApi
    {
        private readonly IPasswordService _passwordService;
        private readonly IAthenaRepository _athenaRepository;

        public AthenaApi(IPasswordService passwordService,
            IAthenaRepository athenaRepository)
        {
            _passwordService = passwordService;
            _athenaRepository = athenaRepository;
        }

        public Task<bool> DoesUsernameExistAsync(string username, CancellationToken cancellationToken = default)
        {
            username = username.ToLower();
            return _athenaRepository.DoesUsernameExistsAsync(username, cancellationToken);
        }

        public async Task<bool> ValidateUserCredentialsByUsernameAsync(string username, string password,
            CancellationToken cancellationToken = default)
        {
            var data = await _athenaRepository.GetUserAccountByUsernameAsync(username, cancellationToken);
            return data != null && _passwordService.VerifyUserAccountPassword(data.Id, data.PasswordHash!, password);
        }

        public async Task<bool> ValidateServiceCodeAsync(Guid serviceId, string authorizationCode,
            CancellationToken cancellationToken = default)
        {
            var data = await _athenaRepository.GetServiceAccountAsync(serviceId, cancellationToken);
            return data != null && _passwordService.VerifyAuthorizationCode(data.AuthorizationCode, authorizationCode);
        }

        public async Task<(string? NewPassword, ResetUserPasswordError? Error)> ResetUserPasswordAsync(Guid userId,
            CancellationToken cancellationToken = default)
        {
            var data = await _athenaRepository.GetUserAccountByIdAsync(userId, cancellationToken);

            if (data is null)
            {
                return (null, ResetUserPasswordError.InvalidPasswordFormat);
            }

            data.PasswordHash = _passwordService.HashPassword(Guid.NewGuid().ToString("D"));

            await _athenaRepository.UpdateUserAsync(data, cancellationToken);

            return (_passwordService.DecodePassword(data.PasswordHash), null);
        }

        public async Task<(PantheonIdentity?, GetValidatedEntityError?)> GetValidatedUserByDeviceIdAsync(
            string deviceId,
            string password,
            CancellationToken cancellationToken = default)
        {
            var identity = await _athenaRepository.GetUserAccountByDeviceIdAsync(deviceId, cancellationToken);
            return GetValidatedIdentityAsync(password, identity);
        }


        public async Task<(PantheonIdentity?, GetValidatedEntityError?)> GetValidatedUserByUsernameAsync(
            string username,
            string password,
            CancellationToken cancellationToken = default)
        {
            var identity = await _athenaRepository.GetUserAccountByUsernameAsync(username, cancellationToken);
            return GetValidatedIdentityAsync(password, identity);
        }

        private (PantheonIdentity?, GetValidatedEntityError?) GetValidatedIdentityAsync(string password,
            UserAccountDataModel? identity)
        {
            if (identity is null) return (null, GetValidatedEntityError.IdentityNotFound);

            if (_passwordService.VerifyUserAccountPassword(identity.Id, identity.PasswordHash!, password))
                return (IdentityMappers.ToDomain(identity), null);

            return (null, GetValidatedEntityError.InvalidCredentials);
        }

        public async Task<(PantheonService?, GetValidatedEntityError?)> GetValidatedServiceAsync(Guid serviceId,
            string authorizationCode,
            CancellationToken cancellationToken = default)
        {
            var data = await GetServiceAccountByIdAsync(serviceId, cancellationToken);
            if (data is null) return (null, GetValidatedEntityError.IdentityNotFound);

            if (_passwordService.VerifyAuthorizationCode(data.AuthorizationCode, authorizationCode))
                return (data, null);

            return (null, GetValidatedEntityError.InvalidCredentials);
        }

        private async Task<PantheonService?> GetServiceAccountByIdAsync(Guid serviceId,
            CancellationToken cancellationToken)
        {
            var data = await _athenaRepository.GetServiceAccountAsync(serviceId, cancellationToken);

            return data == null ? null : ServiceMappers.ToDomain(data);
        }
    }
}