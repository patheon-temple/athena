namespace Athena.Gate.MongoDb;

public interface IAthenaMongoCollectionSettings
{
    string UserAccountsCollectionName { get; }
    string ServiceAccountsCollectionName { get; }
}