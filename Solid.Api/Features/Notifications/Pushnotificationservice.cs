using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

namespace Solid.Api.Features.Notifications;

public sealed class PushNotificationService : IPushNotificationService
{
    private readonly FirebaseMessaging? _messaging;
    private readonly ILogger<PushNotificationService> _logger;

    public PushNotificationService(IConfiguration configuration, ILogger<PushNotificationService> logger)
    {
        _logger = logger;
        _messaging = TryInitializeMessaging(configuration);
    }

    public async Task SendAsync(string? fcmToken, string title, string body, IDictionary<string, string>? data = null)
    {
        if (_messaging is null || string.IsNullOrWhiteSpace(fcmToken))
        {
            return;
        }

        var message = new Message
        {
            Token = fcmToken,
            Notification = new FirebaseAdmin.Messaging.Notification
            {
                Title = title,
                Body = body
            },
            // ✅ الفيكس: نمرر Dictionary<string,string> صريح، مش IDictionary<string,string>
            // لأن Message.Data متعرّفة كـ IReadOnlyDictionary<string,string> في FirebaseAdmin SDK
            Data = data is null ? null : new Dictionary<string, string>(data)
        };

        try
        {
            await _messaging.SendAsync(message);
        }
        catch (FirebaseMessagingException ex)
        {
            _logger.LogWarning(ex, "Failed to send push notification to token {Token}", fcmToken);
        }
    }

    public async Task SendToManyAsync(IEnumerable<string?> fcmTokens, string title, string body, IDictionary<string, string>? data = null)
    {
        if (_messaging is null)
        {
            return;
        }

        var tokens = fcmTokens
            .Where(token => !string.IsNullOrWhiteSpace(token))
            .Select(token => token!)
            .Distinct()
            .ToList();

        if (tokens.Count == 0)
        {
            return;
        }

        var multicastMessage = new MulticastMessage
        {
            Tokens = tokens,
            Notification = new FirebaseAdmin.Messaging.Notification
            {
                Title = title,
                Body = body
            },
            Data = data is null ? null : new Dictionary<string, string>(data)
        };

        try
        {
            await _messaging.SendEachForMulticastAsync(multicastMessage);
        }
        catch (FirebaseMessagingException ex)
        {
            _logger.LogWarning(ex, "Failed to send multicast push notification.");
        }
    }

    private FirebaseMessaging? TryInitializeMessaging(IConfiguration configuration)
    {
        try
        {
            if (FirebaseApp.DefaultInstance is null)
            {
                var projectId = configuration["Firebase:ProjectId"];
                var clientEmail = configuration["Firebase:ClientEmail"];
                var privateKey = configuration["Firebase:PrivateKey"];

                if (string.IsNullOrWhiteSpace(projectId) ||
                    string.IsNullOrWhiteSpace(clientEmail) ||
                    string.IsNullOrWhiteSpace(privateKey))
                {
                    _logger.LogWarning("Firebase credentials are not configured. Push notifications are disabled.");
                    return null;
                }

                var credentialParameters = new JsonCredentialParameters
                {
                    Type = JsonCredentialParameters.ServiceAccountCredentialType,
                    ProjectId = projectId,
                    ClientEmail = clientEmail,
                    PrivateKey = privateKey.Replace("\\n", "\n")
                };

                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromJsonParameters(credentialParameters),
                    ProjectId = projectId
                });
            }

            return FirebaseMessaging.DefaultInstance;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Firebase push notifications.");
            return null;
        }
    }
}
