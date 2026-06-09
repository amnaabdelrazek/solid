using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Solid.Api.Infrastructure.Jitsi;

public interface IJitsiTokenService
{
    string Create(long userId, string displayName, string roomName, bool isModerator);
}

public sealed class JitsiTokenService(IConfiguration configuration) : IJitsiTokenService
{
    public string Create(long userId, string displayName, string roomName, bool isModerator)
    {
        var appId = configuration["Jitsi:AppId"]
            ?? throw new InvalidOperationException("Jitsi:AppId is required.");
        var appSecret = configuration["Jitsi:AppSecret"]
            ?? throw new InvalidOperationException("Jitsi:AppSecret is required.");
        var serverUrl = configuration["Jitsi:ServerUrl"]
            ?? throw new InvalidOperationException("Jitsi:ServerUrl is required.");

        var domain = new Uri(serverUrl).Host;

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSecret)),
            SecurityAlgorithms.HmacSha256);

        var contextJson = System.Text.Json.JsonSerializer.Serialize(new
        {
            user = new
            {
                id = userId.ToString(),
                name = displayName,
                moderator = isModerator
            },
            features = new
            {
                livestreaming = false,
                outbound_call = false,
                transcription = false,
                recording = isModerator
            }
        });

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, "jitsi-key"),
            new(JwtRegisteredClaimNames.Iss, appId),
            new(JwtRegisteredClaimNames.Aud, domain),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new("room", roomName),
            new("context", contextJson, JsonClaimValueTypes.Json)
        };

        var token = new JwtSecurityToken(
            issuer: appId,
            audience: domain,
            claims: claims,
            notBefore: DateTime.UtcNow.AddSeconds(-5),
            expires: DateTime.UtcNow.AddHours(3),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
