using System.Security.Claims;

namespace Solid.Api.Infrastructure.Auth;

public sealed class JwtAuthContext(IHttpContextAccessor httpContextAccessor) : IAuthContext
{
    public long UserId
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            return long.TryParse(value, out var userId) ? userId : 0;
        }
    }

    public string? Role => httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
}
