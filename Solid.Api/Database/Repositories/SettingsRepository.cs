using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Solid.Api.Database.Entities;

namespace Solid.Api.Database.Repositories;

public sealed class SettingsRepository(SolidDbContext dbContext) : ISettingsRepository
{
    public async Task<string?> GetAsync(string group, string name)
    {
        return await dbContext.Settings
            .AsNoTracking()
            .Where(setting => setting.Group == group && setting.Name == name)
            .Select(setting => setting.Payload)
            .FirstOrDefaultAsync();
    }

    public async Task SetAsync(string group, string name, object? value)
    {
        var payload = JsonSerializer.Serialize(value);
        var setting = await dbContext.Settings
            .FirstOrDefaultAsync(setting => setting.Group == group && setting.Name == name);

        var now = DateTime.UtcNow;
        if (setting is null)
        {
            dbContext.Settings.Add(new Setting
            {
                Group = group,
                Name = name,
                Locked = false,
                Payload = payload,
                CreatedAt = now,
                UpdatedAt = now
            });
        }
        else
        {
            setting.Payload = payload;
            setting.UpdatedAt = now;
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task SetRawAsync(string group, string name, string value)
    {
        var setting = await dbContext.Settings
            .FirstOrDefaultAsync(setting => setting.Group == group && setting.Name == name);

        var now = DateTime.UtcNow;
        if (setting is null)
        {
            dbContext.Settings.Add(new Setting
            {
                Group = group,
                Name = name,
                Locked = false,
                Payload = value,
                CreatedAt = now,
                UpdatedAt = now
            });
        }
        else
        {
            setting.Payload = value;
            setting.UpdatedAt = now;
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Notification>> NotificationsAsync(long userId)
    {
        return await dbContext.Notifications
            .AsNoTracking()
            .Where(notification => notification.NotifiableId == userId && notification.DeletedAt == null)
            .OrderByDescending(notification => notification.CreatedAt)
            .Take(20)
            .ToListAsync();
    }

    // جديد — تعليم نوتيفيكيشن واحدة كمقروءة
    public async Task<bool> MarkNotificationReadAsync(long userId, Guid notificationId)
    {
        var notification = await dbContext.Notifications
            .FirstOrDefaultAsync(n =>
                n.Id == notificationId &&
                n.NotifiableId == userId &&
                n.DeletedAt == null);

        if (notification is null)
        {
            return false;
        }

        var now = DateTime.UtcNow;
        notification.ReadAt ??= now;
        notification.UpdatedAt = now;

        await dbContext.SaveChangesAsync();

        return true;
    }

    // جديد — تعليم كل نوتيفيكيشن اليوزر كمقروءة
    public async Task<int> MarkAllNotificationsReadAsync(long userId)
    {
        var now = DateTime.UtcNow;

        return await dbContext.Notifications
            .Where(n => n.NotifiableId == userId && n.DeletedAt == null && n.ReadAt == null)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(n => n.ReadAt, now)
                .SetProperty(n => n.UpdatedAt, now));
    }

    // جديد — Soft delete لنوتيفيكيشن واحدة
    public async Task<bool> SoftDeleteNotificationAsync(long userId, Guid notificationId)
    {
        var notification = await dbContext.Notifications
            .FirstOrDefaultAsync(n =>
                n.Id == notificationId &&
                n.NotifiableId == userId &&
                n.DeletedAt == null);

        if (notification is null)
        {
            return false;
        }

        var now = DateTime.UtcNow;
        notification.DeletedAt = now;
        notification.UpdatedAt = now;

        await dbContext.SaveChangesAsync();

        return true;
    }

    // جديد — Soft delete لكل نوتيفيكيشن اليوزر
    public async Task<int> SoftDeleteAllNotificationsAsync(long userId)
    {
        var now = DateTime.UtcNow;

        return await dbContext.Notifications
            .Where(n => n.NotifiableId == userId && n.DeletedAt == null)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(n => n.DeletedAt, now)
                .SetProperty(n => n.UpdatedAt, now));
    }
}
