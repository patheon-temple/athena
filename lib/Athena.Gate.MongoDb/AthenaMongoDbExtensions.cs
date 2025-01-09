using Athena.SDK.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Athena.Gate.MongoDb;


public static class AthenaMongoDbExtensions
{
    
    public static void AddAthenaMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUserRepository, AthenaMongoDbRepository>();
        services.AddScoped<IServiceRepository, AthenaMongoDbRepository>();
        var connectionString = configuration.GetConnectionString("MongoDb") ?? throw new NullReferenceException("MongoDb connection string is not configured in appsettings.json");
        services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
    }
}