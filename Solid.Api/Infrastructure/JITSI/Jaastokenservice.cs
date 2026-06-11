using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Solid.Api.Infrastructure.Jitsi;

public interface IJaasTokenService
{
    string Create(long userId, string displayName, string? avatarUrl, string roomName, bool isModerator);
}

/// <summary>
/// Generates JaaS (8x8.vc) JWT tokens.
/// Docs: https://developer.8x8.com/jaas/docs/api-keys-jwt
///
/// appsettings required:
///   Jaas:AppId       → your JaaS App ID  (e.g. "vpaas-magic-cookie-abc123")
///   Jaas:KeyId       → your API Key ID   (e.g. "vpaas-magic-cookie-abc123/abcdef")
///   Jaas:PrivateKey  → RSA private key PEM (multiline, or set via env var)
/// </summary>
public sealed class JaasTokenService(IConfiguration configuration) : IJaasTokenService
{
    public string Create(
        long userId,
        string displayName,
        string? avatarUrl,
        string roomName,
        bool isModerator)
    {
        var appId = configuration["Jaas:AppId"]
            ?? throw new InvalidOperationException("Jaas:AppId is required.");

        var keyId = configuration["Jaas:KeyId"]
            ?? throw new InvalidOperationException("Jaas:KeyId is required.");

        var privateKeyPem = configuration["Jaas:PrivateKey"]
            ?? throw new InvalidOperationException("Jaas:PrivateKey is required.");

        // Load RSA private key from PEM
        using var rsa = RSA.Create();
        rsa.ImportFromPem(privateKeyPem.ToCharArray());

        var securityKey = new RsaSecurityKey(rsa.ExportParameters(true))
        {
            KeyId = keyId
        };

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);

        // JaaS context claim — user info + features
        var contextJson = System.Text.Json.JsonSerializer.Serialize(new
        {
            user = new
            {
                id = userId.ToString(),
                name = displayName,
                avatar = avatarUrl ?? string.Empty,
                email = string.Empty,
                moderator = isModerator.ToString().ToLower()   // "true" / "false"  (string, not bool)
            },
            features = new
            {
                recording = isModerator,
                livestreaming = false,
                transcription = false,
                outbound_call = false
            }
        });

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Iss, "chat"),
            new(JwtRegisteredClaimNames.Aud, "jitsi"),
            new(JwtRegisteredClaimNames.Sub, appId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new("room", roomName),
            new("context", contextJson, JsonClaimValueTypes.Json)
        };

        var now = DateTime.UtcNow;

        var token = new JwtSecurityToken(
            issuer: "chat",
            audience: "jitsi",
            claims: claims,
            notBefore: now.AddSeconds(-10),   // small clock-skew buffer
            expires: now.AddHours(3),
            signingCredentials: credentials);

        // JaaS requires kid header = KeyId
        token.Header["kid"] = keyId;

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
