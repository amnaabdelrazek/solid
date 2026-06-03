using Solid.Api.Features.Shared;

namespace Solid.Api.Database.Repositories;

public sealed class SettingsRepository(IDatabase database) : ISettingsRepository
{
    public async Task<string?> GetAsync(string group, string name)
    {
        return await database.SettingAsync(group, name);
    }

    public async Task SetAsync(string group, string name, object? value)
    {
        var payload = System.Text.Json.JsonSerializer.Serialize(value);
        await database.ExecuteAsync(
            """
            MERGE settings AS target
            USING (SELECT @group AS [group], @name AS [name]) AS source
            ON target.[group] = source.[group] AND target.[name] = source.[name]
            WHEN MATCHED THEN UPDATE SET payload = @payload, updated_at = SYSDATETIME()
            WHEN NOT MATCHED THEN INSERT ([group], [name], locked, payload, created_at, updated_at)
            VALUES (@group, @name, 0, @payload, SYSDATETIME(), SYSDATETIME());
            """,
            new { group, name, payload });
    }

    public async Task<IReadOnlyList<Dictionary<string, object?>>> NotificationsAsync(long userId)
    {
        return await database.QueryAsync(
            """
            SELECT TOP 20 *
            FROM notifications
            WHERE notifiable_id = @userId
            ORDER BY created_at DESC
            """,
            new { userId });
    }
}
