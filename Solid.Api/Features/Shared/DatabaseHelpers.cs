using System.Text;
using Solid.Api.Common;
using Solid.Api.Database;

namespace Solid.Api.Features.Shared;

public static class DatabaseHelpers
{
    public static async Task<Dictionary<string, object?>?> UserAsync(this IDatabase database, long userId)
    {
        return await database.QuerySingleAsync("SELECT TOP 1 * FROM users WHERE id = @userId AND deleted_at IS NULL", new { userId });
    }

    public static async Task<(long Id, string PlainText)> CreateSanctumTokenAsync(this IDatabase database, long userId, string name)
    {
        var plain = Hashing.RandomToken();
        var hash = Hashing.Sha256(plain);
        var tokenId = await database.ExecuteScalarAsync<long>(
            """
            INSERT INTO personal_access_tokens (tokenable_type, tokenable_id, [name], token, abilities, created_at, updated_at)
            OUTPUT INSERTED.id
            VALUES (@type, @userId, @name, @hash, '["*"]', SYSDATETIME(), SYSDATETIME())
            """,
            new { type = Constants.LaravelUserType, userId, name, hash });

        return (tokenId, $"{tokenId}|{plain}");
    }

    public static async Task<string?> SettingAsync(this IDatabase database, string group, string name)
    {
        var row = await database.QuerySingleAsync("SELECT TOP 1 payload FROM settings WHERE [group] = @group AND [name] = @name", new { group, name });

        return row is null ? null : Convert.ToString(row["payload"])?.Trim('"');
    }

    public static async Task PutCacheAsync(this IDatabase database, string key, string value, int seconds)
    {
        var payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
        var expiration = DateTimeOffset.UtcNow.AddSeconds(seconds).ToUnixTimeSeconds();
        await database.ExecuteAsync(
            """
            MERGE cache AS target
            USING (SELECT @key AS [key]) AS source
            ON target.[key] = source.[key]
            WHEN MATCHED THEN UPDATE SET [value] = @payload, expiration = @expiration
            WHEN NOT MATCHED THEN INSERT ([key], [value], expiration) VALUES (@key, @payload, @expiration);
            """,
            new { key, payload, expiration });
    }

    public static async Task<string?> GetCacheAsync(this IDatabase database, string key)
    {
        var row = await database.QuerySingleAsync("SELECT TOP 1 [value] FROM cache WHERE [key] = @key AND expiration > @now", new { key, now = DateTimeOffset.UtcNow.ToUnixTimeSeconds() });
        if (row is null)
        {
            return null;
        }

        return Encoding.UTF8.GetString(Convert.FromBase64String(Convert.ToString(row["value"])!));
    }
}
