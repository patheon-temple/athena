using Athena.Gate.MongoDb.Mappers;
using Athena.Gate.MongoDb.Models;
using Athena.SDK.Models;
using Athena.SDK.Services;
using MongoDB.Driver;

public interface IAthenaMongoCollectionSettings
{
    string UserAccountsCollectionName { get; }
    string ServiceAccountsCollectionName { get; }
}

public class AthenaMongoDbRepository : IUserRepository, IServiceRepository, IDisposable
{
    private readonly IMongoCollection<UserAccountMongoDataModel> _userAccounts;
    private readonly IMongoCollection<ServiceAccountMongoDataModel> _serviceAccounts;

    public AthenaMongoDbRepository(IAthenaMongoCollectionSettings settings, IMongoDatabase database)
    {
        _userAccounts = database.GetCollection<UserAccountMongoDataModel>(settings.UserAccountsCollectionName);
        _serviceAccounts = database.GetCollection<ServiceAccountMongoDataModel>(settings.ServiceAccountsCollectionName);
    }

    public void Dispose()
    {
        
    }

    public async Task<PantheonUser?> GetUserAccountByDeviceIdAsync(string deviceId, CancellationToken cancellationToken)
    {
        var filter = Builders<UserAccountMongoDataModel>.Filter.Eq(x => x.DeviceId, deviceId);

        var data = await _userAccounts.Find(filter).FirstOrDefaultAsync(cancellationToken);

        return data == null ? null : UserMappers.ToDomain(data);
    }

    public async Task<PantheonUser> CreateUserAccountAsync(PantheonUser user, CancellationToken cancellationToken)
    {
        var data = UserMappers.ToDataModel(user);
        await _userAccounts.InsertOneAsync(data, new InsertOneOptions(), cancellationToken);
        return UserMappers.ToDomain(data);
    }

    public Task<bool> DoesUsernameExistsAsync(string username, CancellationToken cancellationToken)
    {
        var filter = Builders<UserAccountMongoDataModel>.Filter.Eq(x => x.Username, username);
        return _userAccounts.Find(filter).AnyAsync(cancellationToken);
    }

    public async Task<PantheonUser?> GetUserAccountByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        var filter = Builders<UserAccountMongoDataModel>.Filter.Eq(x => x.Username, username);

        var data = await _userAccounts.Find(filter).FirstOrDefaultAsync(cancellationToken);

        return data == null ? null : UserMappers.ToDomain(data);
    }

    public async Task<PantheonUser?> GetUserAccountByIdAsync(string id, CancellationToken cancellationToken)
    {
        var filter = Builders<UserAccountMongoDataModel>.Filter.Eq(x => x.Id, id);

        var data = await _userAccounts.Find(filter).FirstOrDefaultAsync(cancellationToken);

        return data == null ? null : UserMappers.ToDomain(data);
    }

    public async Task UpdateUserAsync(PantheonUser user, CancellationToken cancellationToken = default)
    {
        var data = UserMappers.ToDataModel(user);
        var filter = Builders<UserAccountMongoDataModel>.Filter.Eq(x => x.Id, user.Id);
        await _userAccounts.ReplaceOneAsync(filter, data, new ReplaceOptions { IsUpsert = true }, cancellationToken);
    }

    public async Task<PantheonService?> GetServiceAccountByIdAsync(string serviceId,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<ServiceAccountMongoDataModel>.Filter.Eq(x => x.Id, serviceId);

        var data = await _serviceAccounts.Find(filter).FirstOrDefaultAsync(cancellationToken);

        return data == null ? null : ServiceMappers.ToDomain(data);
    }

    public async Task<PantheonService> CreateServiceAccountAsync(PantheonService service,
        CancellationToken cancellationToken)
    {
        var data = ServiceMappers.ToDataModel(service);
        await _serviceAccounts.InsertOneAsync(data, new InsertOneOptions(), cancellationToken);
        return ServiceMappers.ToDomain(data);
    }

    public Task<bool> DoesServiceAccountWithIdExistsAsync(string serviceId, CancellationToken cancellationToken)
    {
        var filter = Builders<ServiceAccountMongoDataModel>.Filter.Eq(x => x.Id, serviceId);
        return _serviceAccounts.Find(filter).AnyAsync(cancellationToken);
    }
}