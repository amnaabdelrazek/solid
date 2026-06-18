using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Solid.Api.Database;
using Solid.Api.Database.Entities;
using Solid.Api.Features.Settings;

namespace Solid.Api.Features.Notifications;

public sealed class NotificationService(
    SolidDbContext dbContext,
    IHubContext<NotificationsHub> hubContext) : INotificationService
{
    public async Task NotifyUsersAsync(
        IEnumerable<long> userIds,
        string type,
        string title,
        string body,
        string icon,
        object? data = null)
    {
        var recipients = userIds
            .Where(userId => userId > 0)
            .Distinct()
            .ToArray();

        if (recipients.Length == 0)
        {
            return;
        }

        var now = DateTime.UtcNow;
        var payload = JsonSerializer.Serialize(new
        {
            title,
            body,
            type,
            icon,
            data
        });

        var notifications = recipients
            .Select(userId => new Notification
            {
                Id = Guid.NewGuid(),
                Type = $"App\\Notifications\\{type}",
                NotifiableType = "Modules\\User\\Models\\User",
                NotifiableId = userId,
                Data = payload,
                CreatedAt = now,
                UpdatedAt = now
            })
            .ToArray();

        dbContext.Notifications.AddRange(notifications);
        await dbContext.SaveChangesAsync();

        foreach (var notification in notifications)
        {
            await hubContext
                .Clients
                .Group($"user:{notification.NotifiableId}")
                .SendAsync("notification", NotificationResource.From(notification));
        }
    }
}
