namespace Solid.Api.Infrastructure.Auth;

public interface IJwtTokenRevocationService
{
    Task RevokeAsync(string token);

    Task<bool> IsRevokedAsync(string token);
}
