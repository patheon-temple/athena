using Athena.SDK;
using Athena.SDK.Crypto;
using Athena.SDK.Domain;
using Athena.SDK.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Athena.Gate.WebApi;

public static class ServiceInstaller
{
    public static void InstallCoreServices(IServiceCollection services)
    {
        services.AddScoped<IAthenaApi, AthenaApi>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IAthenaAdminApi, AthenaAdminApi>();

        services.AddOptions<AthenaConfiguration>().BindConfiguration(
                "AthenaConfiguration")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddTransient<IValidator<UserCredentialsValidationParams>>(x => Passwords.CreateValidator());
    }

#if ATHENA_POSTGRES
    public static void InstallPostgresServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAthenaRepository, AthenaRepository>();
        services.AddPooledDbContextFactory<AthenaDbContext>(opt =>
            opt.UseNpgsql(
                configuration.GetConnectionString(GlobalDefinitions.ConfigurationKeys.PostgresConnectionString),
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "athena")));
    }
#endif
}