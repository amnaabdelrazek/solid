using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Solid.Api.Database;
using Solid.Api.Database.Entities;
using Solid.Api.Features.Settings;

namespace Solid.Api.Features.Notifications;

public sealed class NotificationService(
    SolidDbContext dbContext,
    IHubContext<NotificationsHub> hubContext,
    IPushNotificationService pushNotificationService) : INotificationService
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

        // 🔔 إرسال Push Notification عبر Firebase لأي يوزر معاه fcm_token مسجل
        await SendPushNotificationsAsync(recipients, type, title, body, data);
    }

    private async Task SendPushNotificationsAsync(
        long[] recipients,
        string type,
        string title,
        string body,
        object? data)
    {
        var fcmTokens = await dbContext.Users
            .AsNoTracking()
            .Where(user => recipients.Contains(user.Id) && user.FcmToken != null)
            .Select(user => user.FcmToken!)
            .ToListAsync();

        if (fcmTokens.Count == 0)
        {
            return;
        }

        var pushData = BuildPushData(type, data);

        await pushNotificationService.SendToManyAsync(fcmTokens, title, body, pushData);
    }

    private static Dictionary<string, string> BuildPushData(string type, object? data)
    {
        var result = new Dictionary<string, string> { ["type"] = type };

        if (data is null)
        {
            return result;
        }

        var dataJson = JsonSerializer.Serialize(data);
        var dataDictionary = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(dataJson);

        if (dataDictionary is null)
        {
            return result;
        }

        foreach (var (key, value) in dataDictionary)
        {
            result[key] = value.ValueKind switch
            {
                JsonValueKind.String => value.GetString() ?? string.Empty,
                JsonValueKind.Null => string.Empty,
                _ => value.ToString()
            };
        }

        return result;
    }
}
