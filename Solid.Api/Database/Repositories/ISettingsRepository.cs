using Solid.Api.Database.Entities;

namespace Solid.Api.Database.Repositories;

public interface ISettingsRepository
{
    Task<string?> GetAsync(string group, string name);

    Task SetAsync(string group, string name, object? value);

    Task SetRawAsync(string group, string name, string value);

    Task<IReadOnlyList<Notification>> NotificationsAsync(long userId);

    // جديد
    Task<bool> MarkNotificationReadAsync(long userId, Guid notificationId);
    Task<int> MarkAllNotificationsReadAsync(long userId);   // جديد


    Task<bool> SoftDeleteNotificationAsync(long userId, Guid notificationId);

    Task<int> SoftDeleteAllNotificationsAsync(long userId);
}
