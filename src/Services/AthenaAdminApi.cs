using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Athena.SDK;
using Athena.SDK.Crypto;
using Athena.SDK.Data;
using Athena.SDK.Definitions;
using Athena.SDK.Mappers;
using Athena.SDK.Models;
using Athena.SDK.Services;
using FluentValidation;


internal sealed class AthenaAdminApi : IAthenaAdminApi
{
    private readonly IAthenaApi _athenaApi;
    private readonly IAthenaRepository _athenaRepository;

    public AthenaAdminApi(IAthenaRepository athenaRepository, IAthenaApi athenaApi)
    {
        _athenaRepository = athenaRepository;
        _athenaApi = athenaApi;
    }

    public async Task<PantheonIdentity> CreateUserAsync(
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

        var scopeEntities = await _athenaRepository.GetUserScopesAsync(scopes, cancellationToken);

        var identity = new UserAccountDataModel
        {
            DeviceId = deviceId,
            PasswordHash = string.IsNullOrWhiteSpace(password) ? null : Passwords.HashPassword(password),
            Username = username?.ToLower(),
            Scopes = scopeEntities
        };
        await _athenaRepository.CreateUserAccountAsync(identity, cancellationToken);

        return IdentityMappers.ToDomain(identity);
    }

    public async Task<PantheonIdentity?> GetUserAccountByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var data = await _athenaRepository.GetUserAccountByIdAsync(id, cancellationToken);
        return data is null ? null : IdentityMappers.ToDomain(data);
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

        var scopes = await _athenaRepository.GetServiceAccountScopesAsync(requiredScopes, cancellationToken);

        var salt = new byte[512];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);

        var serviceAccountDataModel = new ServiceAccountDataModel
        {
            Id = Guid.NewGuid(),
            Name = serviceName,
            AuthorizationCode = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")),
            Scopes = scopes,
        };
        var entity = await _athenaRepository.CreateServiceAccountAsync(serviceAccountDataModel, cancellationToken);
        return ServiceMappers.ToDomain(entity);
    }


    public async Task<PantheonService?> GetServiceAccountByIdAsync(Guid serviceId,
        CancellationToken cancellationToken = default)
    {
        var data = await _athenaRepository.GetServiceAccountAsync(serviceId, cancellationToken);

        return data == null ? null : ServiceMappers.ToDomain(data);
    }

    public Task<bool> ServiceAccountExistsAsync(Guid serviceId, CancellationToken cancellationToken = default)
    {
        return _athenaRepository.HasServiceAccountWithIdAsync(serviceId, cancellationToken);
    }
}