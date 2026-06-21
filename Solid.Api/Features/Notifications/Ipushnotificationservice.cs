namespace Solid.Api.Features.Notifications;

public interface IPushNotificationService
{
    Task SendAsync(string? fcmToken, string title, string body, IDictionary<string, string>? data = null);

    Task SendToManyAsync(IEnumerable<string?> fcmTokens, string title, string body, IDictionary<string, string>? data = null);
}
