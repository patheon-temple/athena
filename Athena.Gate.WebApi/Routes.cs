using Athena.SDK.Definitions;
using Microsoft.AspNetCore.Builder;

namespace Athena.Gate.WebApi;

public static class Routes
{
    public static void Register(WebApplication app)
    {
        var builder = app.NewVersionedApi("Athena");
        var v1 = builder.MapGroup("/athena/api/v{v:apiVersion}").HasApiVersion(1.0);

        v1.MapPatch("account/user/password", Handlers.ResetUserPassword)
            .RequireAuthorization();

        var v1Admin = builder.MapGroup("/athena/api/admin/v{v:apiVersion}").HasApiVersion(1.0);
        v1Admin.MapPost("account/user", Handlers.CreateUserAccount)
            .RequireAuthorization(GlobalDefinitions.Policies.SuperUser);
        v1Admin.MapPost("account/service", Handlers.CreateServiceAccountAsync)
            .RequireAuthorization(GlobalDefinitions.Policies.SuperUser);

        v1Admin.MapGet("account/user/{id:guid}", Handlers.GetUserAccountByIdAsync)
            .WithName(nameof(Handlers.GetUserAccountByIdAsync)) // (mobert): added due to Results.Create routing
            .RequireAuthorization(GlobalDefinitions.Policies.SuperUser);

        v1Admin.MapGet("account/service/{id:guid}", Handlers.GetServiceAccountByIdAsync)
            .WithName(nameof(Handlers.GetServiceAccountByIdAsync)) // (mobert): added due to Results.Create routing
            .RequireAuthorization(GlobalDefinitions.Policies.SuperUser);
    }
}