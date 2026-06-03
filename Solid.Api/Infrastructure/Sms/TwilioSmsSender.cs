using System.Net.Http.Headers;
using System.Text;

namespace Solid.Api.Infrastructure.Sms;

public sealed class TwilioSmsSender(HttpClient httpClient, IConfiguration configuration) : ISmsSender
{
    public async Task SendAsync(string to, string message)
    {
        var accountSid = Value("Sms:Twilio:AccountSid", "TWILIO_SID");
        var authToken = Value("Sms:Twilio:AuthToken", "TWILIO_AUTH_TOKEN");
        var from = Value("Sms:Twilio:From", "TWILIO_FROM");

        if (string.IsNullOrWhiteSpace(accountSid) ||
            string.IsNullOrWhiteSpace(authToken) ||
            string.IsNullOrWhiteSpace(from))
        {
            throw new InvalidOperationException("SMS is not configured. Set Sms:Twilio:AccountSid, Sms:Twilio:AuthToken, and Sms:Twilio:From.");
        }

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"https://api.twilio.com/2010-04-01/Accounts/{accountSid}/Messages.json");

        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{accountSid}:{authToken}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["To"] = to,
            ["From"] = from,
            ["Body"] = message
        });

        using var response = await httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();

            throw new InvalidOperationException($"Twilio SMS failed with {(int)response.StatusCode}: {body}");
        }
    }

    private string? Value(string configKey, string environmentKey)
    {
        return configuration[configKey] ?? Environment.GetEnvironmentVariable(environmentKey);
    }
}
