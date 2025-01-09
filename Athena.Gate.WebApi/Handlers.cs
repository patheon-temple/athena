using System.Text;
using Athena.Gate.WebApi.Interop.Requests;
using Athena.Gate.WebApi.Interop.Responses;
using Athena.Gate.WebApi.Interop.Shared;
using Athena.SDK;
using Athena.SDK.Formatters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Athena.Gate.WebApi;

public static class Handlers
{
    public static async Task<IResult> ResetUserPassword(
        [FromServices] IAthenaApi athenaApi,
        [FromServices] AthenaRequestContext context)
    {
        if (context.UserId == null) return Results.BadRequest();
        var (newPwd, error) =await athenaApi.ResetUserPasswordAsync(GuidFormatters.Stringyfi(context.UserId.Value));

        if (error is not null)
        {
            switch (error)
            {
                case ResetUserPasswordError.InvalidPasswordFormat:
                    return Results.BadRequest("Invalid password format");
                default:
                    throw new Exception(error.ToString());
            }
        }
            
        return Results.Ok(new ResetUserPasswordResponse
        {
            Password = newPwd!
        });
    }

    public static async Task<IResult> GetUserAccountByIdAsync(
        [FromRoute(Name = "id")] string id,
        [FromServices] IAthenaAdminApi adminApi,
        CancellationToken cancellationToken = default)
    {
        var identity = await adminApi.GetUserAccountByIdAsync(id, cancellationToken);
        if (identity is null)
            return Results.NoContent();
        return Results.Ok(new PantheonIdentity(identity));
    }


    public static async Task<IResult> CreateUserAccount(
        [FromBody] CreateUserAccountRequest request,
        [FromServices] IAthenaAdminApi adminApi, CancellationToken cancellationToken = default)
    {
        var identity = await adminApi.CreateUserAsync(request.DeviceId, request.Username, request.Password,
            request.Scopes, cancellationToken);
        return Results.CreatedAtRoute(nameof(GetUserAccountByIdAsync), new { id = identity.Id },
            new PantheonIdentity(identity));
    }

    public static async Task<IResult> CreateServiceAccountAsync(
        [FromBody] CreateServiceAccountRequest request,
        [FromServices] IAthenaAdminApi adminApi, CancellationToken cancellationToken = default)
    {
        var identity = await adminApi.CreateServiceAccountAsync(request.ServiceName, request.Scopes, cancellationToken);
        return Results.CreatedAtRoute(nameof(GetUserAccountByIdAsync), new { id = identity.Id },
            new ServiceAccount
            {
                ServiceName = identity.Name,
                Id = identity.Id,
                AuthorizationCode = Encoding.UTF8.GetString(identity.AuthorizationCode),
            });
    }

    public static async Task<IResult> GetServiceAccountByIdAsync(
        [FromRoute(Name = "id")] string id,
        [FromServices] IAthenaAdminApi adminApi, CancellationToken cancellationToken = default)
    {
        var identity = await adminApi.GetServiceAccountByIdAsync(id, cancellationToken);
        if (identity is null) return Results.NoContent();
        return Results.Ok(new ServiceAccount
        {
            ServiceName = identity.Name,
            Id = identity.Id,
        });
    }
}