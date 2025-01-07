using Athena.Gate.Postgres;
using Athena.SDK.Data;
using Athena.SDK.Definitions;
using Athena.SDK.Domain;
using Athena.SDK.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


public class AthenaRepository(
    IDbContextFactory<AthenaDbContext> contextFactory,
    IOptionsSnapshot<AthenaConfiguration> optionsSnapshot
) : IAthenaRepository
{
    public async Task<UserAccountDataModel?> GetUserAccountByDeviceIdAsync(string deviceId,
        CancellationToken cancellationToken)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await db.UserAccounts.Include(x => x.Scopes)
            .FirstOrDefaultAsync(x => x.DeviceId != null && x.DeviceId.Equals(deviceId),
                cancellationToken: cancellationToken);
    }

    public async Task CreateUserAccountAsync(UserAccountDataModel identity, CancellationToken cancellationToken)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        await db.AddAsync(identity, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DoesUsernameExistsAsync(string username, CancellationToken cancellationToken)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await db.UserAccounts.AnyAsync(x => x.Username != null && x.Username.Equals(username),
            cancellationToken);
    }

    private UserAccountDataModel SuperUser { get; } = new()
    {
        Id = optionsSnapshot.Value.SuperuserId,
        DeviceId = null,
        Username = optionsSnapshot.Value.SuperuserUsername,
        PasswordHash = optionsSnapshot.Value.SuperuserPasswordEncoded,
        Scopes = new List<UserScopeDataModel>
        {
            new()
            {
                Id = GlobalDefinitions.Scopes.Superuser
            }
        }
    };

    public async Task<UserAccountDataModel?> GetUserAccountByUsernameAsync(string username,
        CancellationToken cancellationToken)
    {
        username = username.ToLower();
        if (optionsSnapshot.Value.IsSuperUser(username))
            return SuperUser;

        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await db.UserAccounts.Include(x => x.Scopes)
            .FirstOrDefaultAsync(x => x.Username != null && x.Username.Equals(username),
                cancellationToken: cancellationToken);
    }

    public async Task<UserAccountDataModel?> GetUserAccountByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        if (id.Equals(optionsSnapshot.Value.SuperuserId))
        {
            return SuperUser;
        }

        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await db.UserAccounts.Include(x => x.Scopes)
            .FirstOrDefaultAsync(x => x.Id == id,
                cancellationToken: cancellationToken);
    }

    public async Task UpdateUserAsync(UserAccountDataModel data, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        db.Update(data);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<ServiceAccountDataModel?> GetServiceAccountAsync(Guid serviceId,
        CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await db.ServiceAccounts
            .Include(x => x.Scopes)
            .Where(x => x.Id == serviceId)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }

    public async Task<ICollection<UserScopeDataModel>> GetUserScopesAsync(string[] scopes,
        CancellationToken cancellationToken)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await db.UserScopes
            .Where(x => scopes.Contains(x.Id))
            .ToArrayAsync(cancellationToken: cancellationToken);
    }

    public async Task<ICollection<ServiceScopeDataModel>> GetServiceAccountScopesAsync(string[] requiredScopes,
        CancellationToken cancellationToken)
    {
        if (requiredScopes.Length == 0) return [];
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        return
            await db.ServiceScopes.Where(x => requiredScopes.Contains(x.Id))
                .ToArrayAsync(cancellationToken: cancellationToken);
    }

    public async Task<ServiceAccountDataModel> CreateServiceAccountAsync(ServiceAccountDataModel dataModel,
        CancellationToken cancellationToken)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var data = await db.ServiceAccounts.AddAsync(
            dataModel,
            cancellationToken: cancellationToken);

        await db.SaveChangesAsync(cancellationToken);

        return data.Entity;
    }

    public async Task<bool> HasServiceAccountWithIdAsync(Guid serviceId, CancellationToken cancellationToken)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);

        return await db.ServiceAccounts.AnyAsync(x => x.Id == serviceId, cancellationToken: cancellationToken);
    }
}