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

        services.AddTransient<IValidator<UserCredentialsValidationParams>>(_ => Passwords.CreateValidator());
    }
}