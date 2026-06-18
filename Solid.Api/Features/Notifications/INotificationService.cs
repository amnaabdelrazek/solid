namespace Solid.Api.Features.Notifications;

public interface INotificationService
{
    Task NotifyUsersAsync(
        IEnumerable<long> userIds,
        string type,
        string title,
        string body,
        string icon,
        object? data = null);
}
