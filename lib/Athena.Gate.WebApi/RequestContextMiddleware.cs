using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Athena.Gate.WebApi;

public static class RequestContextMiddleware
{
    public static void UseAthenaRequestContext(this IApplicationBuilder builder)
    {
        builder.Use((context, @delegate) =>
        {
            var principal = context.RequestServices.GetRequiredService<IHttpContextAccessor>()
                .HttpContext?
                .User;
            var identityName = principal?.Identity?
                .Name;

            if (identityName is null || !Guid.TryParse(identityName, out var id)) return @delegate.Invoke();

            var requestContext = context.RequestServices.GetRequiredService<AthenaRequestContext>();
            requestContext.UserId = id;
            return @delegate.Invoke();
        });
    }
}