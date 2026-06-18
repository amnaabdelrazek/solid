using Solid.Api.Database.Entities;

namespace Solid.Api.Database.Repositories;

public interface ISettingsRepository
{
    Task<string?> GetAsync(string group, string name);

    Task SetAsync(string group, string name, object? value);

    Task SetRawAsync(string group, string name, string value);

    Task<IReadOnlyList<Notification>> NotificationsAsync(long userId);
}
