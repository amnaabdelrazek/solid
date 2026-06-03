using Solid.Api.Common;

namespace Solid.Api.Features.Settings;

public static class NotificationResource
{
    public static object From(Dictionary<string, object?> notification)
    {
        var data = notification.JsonValue("data") as Dictionary<string, object?> ?? [];

        return new
        {
            id = notification["id"],
            title = data.GetValueOrDefault("title") ?? "Notification",
            body = data.GetValueOrDefault("body") ?? "",
            type = data.GetValueOrDefault("type") ?? "info",
            icon = data.GetValueOrDefault("icon") ?? "bell",
            read_at = notification.GetValueOrDefault("read_at"),
            created_at = notification.GetValueOrDefault("created_at"),
            created_at_human = ""
        };
    }
}
