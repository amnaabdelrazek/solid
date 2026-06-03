using Microsoft.AspNetCore.Mvc;
using Solid.Api.Common;
using Solid.Api.Features.Shared;
using Solid.Api.Infrastructure.Auth;
using Solid.Api.Database;

namespace Solid.Api.Features.Settings;

public static class SettingsSlice
{
    public static IEndpointRouteBuilder MapSettingsSlice(this IEndpointRouteBuilder api)
    {
        var v1 = api.MapGroup("/v1");

        v1.MapGet("/settings", Index);
        v1.MapGet("/settings/{key}", Show);
        v1.MapPut("/settings/{key}", Update);
        v1.MapGet("/notifications", Notifications);

        return api;
    }

    private static async Task<IResult> Index(IDatabase database)
    {
        var settings = new Dictionary<string, object?>
        {
            ["session_price"] = Setting(await database.SettingAsync("general", "session_price"), "int"),
            ["group_min_members"] = Setting(await database.SettingAsync("general", "group_min_members"), "int"),
            ["group_max_members"] = Setting(await database.SettingAsync("general", "group_max_members"), "int"),
            ["session_duration_minutes"] = Setting(await database.SettingAsync("general", "session_duration_minutes"), "int"),
            ["booking_cutoff_minutes"] = Setting(await database.SettingAsync("general", "booking_cutoff_minutes"), "int"),
            ["session_start_hour"] = Setting(await database.SettingAsync("general", "session_start_hour"), "int"),
            ["session_days"] = Setting(await database.SettingAsync("general", "session_days"), "array"),
            ["auto_start_timeout_minutes"] = Setting(await database.SettingAsync("general", "auto_start_timeout_minutes"), "int")
        };

        return ApiResponse.Ok(new { settings });
    }

    private static async Task<IResult> Show(string key, IDatabase database)
    {
        var value = await database.SettingAsync("general", key);

        return value is null
            ? ApiResponse.Fail($"Setting '{key}' not found.", StatusCodes.Status404NotFound)
            : ApiResponse.Ok(new { setting = new { key, value, type = "string" } });
    }

    private static async Task<IResult> Update(string key, [FromBody] SettingRequest request, IDatabase database)
    {
        var payload = System.Text.Json.JsonSerializer.Serialize(request.value);
        await database.ExecuteAsync(
            """
            MERGE settings AS target
            USING (SELECT 'general' AS [group], @key AS [name]) AS source
            ON target.[group] = source.[group] AND target.[name] = source.[name]
            WHEN MATCHED THEN UPDATE SET payload = @payload, updated_at = SYSDATETIME()
            WHEN NOT MATCHED THEN INSERT ([group], [name], locked, payload, created_at, updated_at)
            VALUES ('general', @key, 0, @payload, SYSDATETIME(), SYSDATETIME());
            """,
            new { key, payload });

        return ApiResponse.Ok(message: "Setting updated successfully.");
    }

    private static async Task<IResult> Notifications(IAuthContext auth, IDatabase database)
    {
        var notifications = await database.QueryAsync(
            """
            SELECT TOP 20 *
            FROM notifications
            WHERE notifiable_id = @userId
            ORDER BY created_at DESC
            """,
            new { auth.UserId });

        return ApiResponse.Ok(new
        {
            notifications = notifications.Select(NotificationResource.From),
            pagination = new
            {
                total = notifications.Count,
                count = notifications.Count,
                per_page = 20,
                current_page = 1,
                total_pages = 1
            }
        });
    }

    private static object Setting(string? value, string type)
    {
        return new { value, type };
    }
}

public sealed record SettingRequest(object? value);
