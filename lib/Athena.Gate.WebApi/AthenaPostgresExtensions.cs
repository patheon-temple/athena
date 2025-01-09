#if ATHENA_POSTGRESQL
using Microsoft.Extensions.DependencyInjection;

namespace Athena.Gate.WebApi;

public static class AthenaPostgresExtensions
{
    public static void InstallPostgresServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAthenaRepository, AthenaRepository>();
        services.AddPooledDbContextFactory<AthenaDbContext>(opt =>
            opt.UseNpgsql(
                configuration.GetConnectionString(GlobalDefinitions.ConfigurationKeys.PostgresConnectionString),
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "athena")));
    }
}
#endif