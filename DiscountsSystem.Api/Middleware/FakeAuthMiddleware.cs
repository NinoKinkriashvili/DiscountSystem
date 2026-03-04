using System.Security.Claims;
using DiscountsSystem.Domain.Enums;

namespace DiscountsSystem.Api.Middleware;

public sealed class FakeAuthMiddleware : IMiddleware
{
    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.User?.Identity?.IsAuthenticated == true)
            return next(context);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "1"),
            new(ClaimTypes.Role, UserRole.Customer.ToString()),
        };

        var identity = new ClaimsIdentity(claims, authenticationType: "Fake");
        context.User = new ClaimsPrincipal(identity);

        return next(context);
    }
}
