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

    public async Task<IReadOnlyList<Notification>> NotificationsAsync(long userId)
    {
        return await dbContext.Notifications
            .AsNoTracking()
            .Where(notification => notification.NotifiableId == userId)
            .OrderByDescending(notification => notification.CreatedAt)
            .Take(20)
            .ToListAsync();
    }
}
