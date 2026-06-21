//using System.Net.Http.Headers;
//using System.Text;
//using System.Text.Json;
//using Solid.Api.Common;
//using Solid.Api.Database.Repositories;

//namespace Solid.Api.Infrastructure.Sms;

//public sealed class InfobipSmsSender(
//    ICacheRepository cacheRepository,
//    IConfiguration configuration,
//    HttpClient httpClient)
//    : ISmsSender
//{
//    public async Task SendOtpAsync(string phoneNumber)
//    {
//        if (!PhoneNumberValidator.TryNormalize(phoneNumber, out var normalizedPhoneNumber))
//        {
//            throw new InvalidOperationException(
//                PhoneNumberValidator.Message);
//        }

//        var otp = Random.Shared
//            .Next(100000, 999999)
//            .ToString();

//        var ttlSeconds =
//            int.TryParse(configuration["Otp:TtlSeconds"], out var ttl)
//                ? ttl
//                : 300;

//        await cacheRepository.PutAsync(
//            $"otp:{normalizedPhoneNumber}",
//            otp,
//            ttlSeconds);

//        var apiKey =
//            configuration["Sms:Infobip:ApiKey"];

//        var baseUrl =
//            configuration["Sms:Infobip:BaseUrl"];

//        var sender =
//            configuration["Sms:Infobip:Sender"] ?? "SOLID";

//        httpClient.DefaultRequestHeaders.Clear();

//        httpClient.DefaultRequestHeaders.Authorization =
//            new AuthenticationHeaderValue("App", apiKey);

//        var payload = new
//        {
//            messages = new[]
//            {
//                new
//                {
//                    from = sender,
//                    destinations = new[]
//                    {
//                        new
//                        {
//                            to = normalizedPhoneNumber
//                        }
//                    },
//                    text = $"Your verification code is: {otp}"
//                }
//            }
//        };

//        var content = new StringContent(
//            JsonSerializer.Serialize(payload),
//            Encoding.UTF8,
//            "application/json");

//        var response = await httpClient.PostAsync(
//            $"{baseUrl}/sms/2/text/advanced",
//            content);

//        var responseBody =
//            await response.Content.ReadAsStringAsync();

//        if (!response.IsSuccessStatusCode)
//        {
//            throw new InvalidOperationException(
//                $"Infobip Error: {responseBody}");
//        }
//    }

//    public async Task<bool> VerifyOtpAsync(
//        string phoneNumber,
//        string otp)
//    {
//        if (!PhoneNumberValidator.TryNormalize(phoneNumber, out var normalizedPhoneNumber))
//        {
//            return false;
//        }

//        var savedOtp = await cacheRepository.GetAsync(
//            $"otp:{normalizedPhoneNumber}");

//        return string.Equals(
//            savedOtp,
//            otp,
//            StringComparison.Ordinal);
//    }
//}


using System.Net.Http.Json;

namespace Solid.Api.Infrastructure.Sms;

public sealed class InfobipSmsSender(HttpClient httpClient) : ISmsSender
{
    public async Task SendAsync(string phoneNumber, string message)
    {
        var request = new
        {
            messages = new[]
            {
                new
                {
                    destinations = new[]
                    {
                        new { to = phoneNumber }
                    },
                    from = "SOLID",
                    text = message
                }
            }
        };

        var response = await httpClient.PostAsJsonAsync(
            "sms/2/text/advanced",
            request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Infobip SMS failed: {error}");
        }
    }
}
