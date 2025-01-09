using System;
using System.Threading;
using System.Threading.Tasks;
using Athena.SDK.Models;

namespace Athena.SDK.Services
{
    public sealed class AthenaApi : IAthenaApi
    {
        private readonly IPasswordService _passwordService;
        private readonly IUserRepository _userRepository;
        private readonly IServiceRepository _serviceRepository;

        public AthenaApi(IPasswordService passwordService, IUserRepository userRepository, IServiceRepository serviceRepository)
        {
            _passwordService = passwordService;
            _userRepository = userRepository;
            _serviceRepository = serviceRepository;
        }

        public Task<bool> DoesUsernameExistAsync(string username, CancellationToken cancellationToken = default)
        {
            username = username.ToLower();
            return _userRepository.DoesUsernameExistsAsync(username, cancellationToken);
        }

        public async Task<bool> ValidateUserCredentialsByUsernameAsync(string username, string password,
            CancellationToken cancellationToken = default)
        {
            var data = await _userRepository.GetUserAccountByUsernameAsync(username, cancellationToken);
            return data != null && _passwordService.VerifyUserAccountPassword(data.Id, data.PasswordHash!, password);
        }

        public async Task<bool> ValidateServiceCodeAsync(string serviceId, string authorizationCode,
            CancellationToken cancellationToken = default)
        {
            var data = await _serviceRepository.GetServiceAccountByIdAsync(serviceId, cancellationToken);
            return data != null && _passwordService.VerifyAuthorizationCode(data.AuthorizationCode, authorizationCode);
        }

        public async Task<(string? NewPassword, ResetUserPasswordError? Error)> ResetUserPasswordAsync(string userId,
            CancellationToken cancellationToken = default)
        {
            var data = await _userRepository.GetUserAccountByIdAsync(userId, cancellationToken);

            if (data is null)
            {
                return (null, ResetUserPasswordError.InvalidPasswordFormat);
            }

            data.PasswordHash = _passwordService.HashPassword(Guid.NewGuid().ToString("D"));

            await _userRepository.UpdateUserAsync(data, cancellationToken);

            return (_passwordService.DecodePassword(data.PasswordHash), null);
        }

        public async Task<(PantheonUser?, GetValidatedEntityError?)> GetValidatedUserByDeviceIdAsync(
            string deviceId,
            string password,
            CancellationToken cancellationToken = default)
        {
            var identity = await _userRepository.GetUserAccountByDeviceIdAsync(deviceId, cancellationToken);
            return GetValidatedIdentityAsync(password, identity);
        }


        public async Task<(PantheonUser?, GetValidatedEntityError?)> GetValidatedUserByUsernameAsync(
            string username,
            string password,
            CancellationToken cancellationToken = default)
        {
            var identity = await _userRepository.GetUserAccountByUsernameAsync(username, cancellationToken);
            return GetValidatedIdentityAsync(password, identity);
        }

        private (PantheonUser?, GetValidatedEntityError?) GetValidatedIdentityAsync(string password,
            PantheonUser? identity)
        {
            if (identity is null) return (null, GetValidatedEntityError.IdentityNotFound);

            if (_passwordService.VerifyUserAccountPassword(identity.Id, identity.PasswordHash!, password))
                return (identity, null);

            return (null, GetValidatedEntityError.InvalidCredentials);
        }

        public async Task<(PantheonService?, GetValidatedEntityError?)> GetValidatedServiceAsync(string serviceId,
            string authorizationCode,
            CancellationToken cancellationToken = default)
        {
            var data = await GetServiceAccountByIdAsync(serviceId, cancellationToken);
            if (data is null) return (null, GetValidatedEntityError.IdentityNotFound);

            if (_passwordService.VerifyAuthorizationCode(data.AuthorizationCode, authorizationCode))
                return (data, null);

            return (null, GetValidatedEntityError.InvalidCredentials);
        }

        private async Task<PantheonService?> GetServiceAccountByIdAsync(string serviceId,
            CancellationToken cancellationToken)
        {
            return await _serviceRepository.GetServiceAccountByIdAsync(serviceId, cancellationToken);
        }
    }
}