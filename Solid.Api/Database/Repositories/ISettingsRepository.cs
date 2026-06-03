namespace Solid.Api.Database.Repositories;

public interface ISettingsRepository
{
    Task<string?> GetAsync(string group, string name);

    Task SetAsync(string group, string name, object? value);

    Task<IReadOnlyList<Dictionary<string, object?>>> NotificationsAsync(long userId);
}
