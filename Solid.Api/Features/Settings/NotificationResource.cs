using Solid.Api.Common;
using Solid.Api.Database.Entities;

namespace Solid.Api.Features.Settings;

public static class NotificationResource
{
    public static object From(Notification notification)
    {
        var data = JsonPayload.ParseObject(notification.Data);

        return new
        {
            id = notification.Id,
            title = data.GetValueOrDefault("title") ?? "Notification",
            body = data.GetValueOrDefault("body") ?? "",
            type = data.GetValueOrDefault("type") ?? "info",
            icon = data.GetValueOrDefault("icon") ?? "bell",
            read_at = notification.ReadAt,
            created_at = notification.CreatedAt,
            created_at_human = ""
        };
    }
}
