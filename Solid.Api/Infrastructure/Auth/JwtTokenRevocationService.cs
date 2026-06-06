using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Solid.Api.Database.Repositories;

namespace Solid.Api.Infrastructure.Auth;

public sealed class JwtTokenRevocationService(ICacheRepository cacheRepository) : IJwtTokenRevocationService
{
    public async Task RevokeAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return;
        }

        var seconds = SecondsUntilExpiry(token);
        if (seconds <= 0)
        {
            return;
        }

        await cacheRepository.PutAsync(CacheKey(token), "revoked", seconds);
    }

    public async Task<bool> IsRevokedAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        return await cacheRepository.GetAsync(CacheKey(token)) is not null;
    }

    private static int SecondsUntilExpiry(string token)
    {
        try
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var seconds = (int)Math.Ceiling((jwt.ValidTo - DateTime.UtcNow).TotalSeconds);

            return seconds > 0 ? seconds : 0;
        }
        catch
        {
            return 0;
        }
    }

    private static string CacheKey(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        var hash = Convert.ToHexString(bytes).ToLowerInvariant();

        return $"revoked_jwt:{hash}";
    }
}
