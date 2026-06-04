using Microsoft.AspNetCore.Mvc;
using Solid.Api.Common;
using Solid.Api.Database.Repositories;
using Solid.Api.Infrastructure.Auth;

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

    private static async Task<IResult> Index(ISettingsRepository settingsRepository)
    {
        var settings = new Dictionary<string, object?>
        {
            ["session_price"] = Setting(await settingsRepository.GetAsync("general", "session_price"), "int"),
            ["group_min_members"] = Setting(await settingsRepository.GetAsync("general", "group_min_members"), "int"),
            ["group_max_members"] = Setting(await settingsRepository.GetAsync("general", "group_max_members"), "int"),
            ["session_duration_minutes"] = Setting(await settingsRepository.GetAsync("general", "session_duration_minutes"), "int"),
            ["booking_cutoff_minutes"] = Setting(await settingsRepository.GetAsync("general", "booking_cutoff_minutes"), "int"),
            ["session_start_hour"] = Setting(await settingsRepository.GetAsync("general", "session_start_hour"), "int"),
            ["session_days"] = Setting(await settingsRepository.GetAsync("general", "session_days"), "array"),
            ["auto_start_timeout_minutes"] = Setting(await settingsRepository.GetAsync("general", "auto_start_timeout_minutes"), "int")
        };

        return ApiResponse.Ok(new { settings });
    }

    private static async Task<IResult> Show(string key, ISettingsRepository settingsRepository)
    {
        var value = await settingsRepository.GetAsync("general", key);

        return value is null
            ? ApiResponse.Fail($"Setting '{key}' not found.", StatusCodes.Status404NotFound)
            : ApiResponse.Ok(new { setting = new { key, value, type = "string" } });
    }

    private static async Task<IResult> Update(string key, [FromBody] SettingRequest request, ISettingsRepository settingsRepository)
    {
        await settingsRepository.SetAsync("general", key, request.value);

        return ApiResponse.Ok(message: "Setting updated successfully.");
    }

    private static async Task<IResult> Notifications(IAuthContext auth, ISettingsRepository settingsRepository)
    {
        var notifications = await settingsRepository.NotificationsAsync(auth.UserId);

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
