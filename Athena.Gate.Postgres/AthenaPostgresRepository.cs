using Athena.Gate.Postgres;
using Athena.Gate.Postgres.Mappers;
using Athena.SDK.Definitions;
using Athena.SDK.Domain;
using Athena.SDK.Models;
using Athena.SDK.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


public class AthenaPostgresRepository(
    IDbContextFactory<AthenaDbContext> contextFactory,
    IOptionsSnapshot<AthenaConfiguration> optionsSnapshot
) : IUserRepository, IServiceRepository, IDisposable, IAsyncDisposable
{
    private AthenaDbContext? _context;

    private async Task<AthenaDbContext> GetDbContextAsync(CancellationToken cancellationToken = default)
    {
        return _context ??= await contextFactory.CreateDbContextAsync(cancellationToken);
    }

    private async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_context != null) await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<PantheonUser?> GetUserAccountByDeviceIdAsync(string deviceId,
        CancellationToken cancellationToken)
    {
        var db = await GetDbContextAsync(cancellationToken);
        var data = await db.UserAccounts.Include(x => x.Scopes)
            .FirstOrDefaultAsync(x => x.DeviceId != null && x.DeviceId.Equals(deviceId),
                cancellationToken: cancellationToken);

        return data == null ? null : IdentityMappers.ToDomain(data);
    }

    public async Task<PantheonUser> CreateUserAccountAsync(PantheonUser user,
        CancellationToken cancellationToken)
    {
        var db = await GetDbContextAsync(cancellationToken);
        var mapper = IdentityMappers.ToDataModel(user);
        var entry = await db.UserAccounts.AddAsync(mapper, cancellationToken);
        await CommitAsync(cancellationToken);

        return IdentityMappers.ToDomain(entry.Entity);
    }

    public async Task<bool> DoesUsernameExistsAsync(string username, CancellationToken cancellationToken)
    {
        var db = await GetDbContextAsync(cancellationToken);
        return await db.UserAccounts.AnyAsync(x => x.Username != null && x.Username.Equals(username),
            cancellationToken);
    }

    private PantheonUser SuperUser { get; } = new()
    {
        Id = optionsSnapshot.Value.SuperuserId,
        DeviceId = null,
        Username = optionsSnapshot.Value.SuperuserUsername,
        PasswordHash = optionsSnapshot.Value.SuperuserPasswordEncoded,
        Scopes = [GlobalDefinitions.Scopes.Superuser]
    };

    public async Task<PantheonUser?> GetUserAccountByUsernameAsync(string username,
        CancellationToken cancellationToken)
    {
        username = username.ToLower();
        if (optionsSnapshot.Value.IsSuperUserUsername(username))
            return SuperUser;

        var db = await GetDbContextAsync(cancellationToken);
        var data = await db.UserAccounts.Include(x => x.Scopes)
            .FirstOrDefaultAsync(x => x.Username != null && x.Username.Equals(username),
                cancellationToken: cancellationToken);

        return data == null ? null : IdentityMappers.ToDomain(data);
    }

    public async Task<PantheonUser?> GetUserAccountByIdAsync(string id, CancellationToken cancellationToken)
    {
        if (id.Equals(optionsSnapshot.Value.SuperuserId))
        {
            return SuperUser;
        }
        var userAccountGuid = Guid.Parse(id);
        var db = await GetDbContextAsync(cancellationToken);
        var data = await db.UserAccounts.Include(x => x.Scopes)
            .FirstOrDefaultAsync(x => x.Id == userAccountGuid,
                cancellationToken: cancellationToken);

        return data == null ? null : IdentityMappers.ToDomain(data);
    }

    public async Task UpdateUserAsync(PantheonUser user, CancellationToken cancellationToken = default)
    {
        var db = await GetDbContextAsync(cancellationToken);
        db.UserAccounts.Update(IdentityMappers.ToDataModel(user));
        await CommitAsync(cancellationToken);
    }

    public async Task<PantheonService?> GetServiceAccountByIdAsync(string serviceId,
        CancellationToken cancellationToken = default)
    {
        var guid = Guid.Parse(serviceId);

        var db = await GetDbContextAsync(cancellationToken);
        var data = await db.ServiceAccounts
            .Include(x => x.Scopes)
            .Where(x => x.Id == guid)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        return data == null ? null : ServiceMappers.ToDomain(data);
    }


    public async Task<PantheonService> CreateServiceAccountAsync(PantheonService service,
        CancellationToken cancellationToken)
    {
        var dataModel = ServiceMappers.ToDataModel(service);
        
        var db = await GetDbContextAsync(cancellationToken);
        
        var entry = await db.ServiceAccounts.AddAsync(
            dataModel,
            cancellationToken: cancellationToken);

        await CommitAsync(cancellationToken);

        return ServiceMappers.ToDomain(entry.Entity);
    }

    public async Task<bool> DoesServiceAccountWithIdExistsAsync(string serviceId, CancellationToken cancellationToken)
    {
        var guid = Guid.Parse(serviceId);

        var db = await GetDbContextAsync(cancellationToken);
        return await db.ServiceAccounts.AnyAsync(x => x.Id == guid, cancellationToken: cancellationToken);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_context != null) await _context.DisposeAsync();
    }
}