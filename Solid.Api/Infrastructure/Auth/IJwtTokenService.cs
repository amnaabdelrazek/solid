namespace Solid.Api.Infrastructure.Auth;

public interface IJwtTokenService
{
    string Create(long userId, string? role, string purpose = "access");
}
