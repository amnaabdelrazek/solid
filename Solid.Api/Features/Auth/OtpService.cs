////using Solid.Api.Database.Repositories;
////using Solid.Api.Infrastructure.Sms;

////namespace Solid.Api.Features.Auth;

////public sealed class OtpService(
////    IAuthRepository authRepository,
////    ISmsSender smsSender,
////    IConfiguration configuration,
////    ILogger<OtpService> logger)
////    : IOtpService
////{
////    public async Task SendRegistrationOtpAsync(
////        string? mobileNumber)
////    {
////        if (string.IsNullOrWhiteSpace(mobileNumber))
////        {
////            throw new InvalidOperationException(
////                "User has no mobile number.");
////        }

////        if (TryUseFixedOtp(out var fixedOtp))
////        {
////            logger.LogWarning("DEV FIXED OTP for registration mobile {MobileNumber}: {Otp}", mobileNumber, fixedOtp);

////            return;
////        }

////        await smsSender.SendOtpAsync(mobileNumber);
////    }

////    public async Task<bool> VerifyRegistrationOtpAsync(
////        string? mobileNumber,
////        string otp)
////    {
////        if (string.IsNullOrWhiteSpace(mobileNumber))
////        {
////            return false;
////        }

////        if (TryUseFixedOtp(out var fixedOtp))
////        {
////            return string.Equals(otp, fixedOtp, StringComparison.Ordinal);
////        }

////        return await smsSender.VerifyOtpAsync(
////            mobileNumber,
////            otp);
////    }

////    public async Task SendPasswordResetOtpAsync(
////        long userId,
////        string? mobileNumber)
////    {
////        if (string.IsNullOrWhiteSpace(mobileNumber))
////        {
////            throw new InvalidOperationException(
////                "User has no mobile number.");
////        }

////        if (TryUseFixedOtp(out var fixedOtp))
////        {
////            logger.LogWarning("DEV FIXED OTP for password reset user {UserId}: {Otp}", userId, fixedOtp);

////            return;
////        }

////        await smsSender.SendOtpAsync(mobileNumber);
////    }

////    public async Task<bool> VerifyPasswordResetOtpAsync(
////        string userId,
////        string otp)
////    {
////        if (!long.TryParse(userId, out var id))
////        {
////            return false;
////        }

////        var user = await authRepository.FindUserByIdAsync(id);

////        if (user is null)
////        {
////            return false;
////        }

////        if (string.IsNullOrWhiteSpace(user.MobileNumber))
////        {
////            return false;
////        }

////        if (TryUseFixedOtp(out var fixedOtp))
////        {
////            return string.Equals(otp, fixedOtp, StringComparison.Ordinal);
////        }

////        return await smsSender.VerifyOtpAsync(
////            user.MobileNumber,
////            otp);
////    }

////    private bool TryUseFixedOtp(out string fixedOtp)
////    {
////        fixedOtp = configuration["Otp:FixedCode"] ?? string.Empty;

////        return configuration.GetValue<bool>("Otp:UseFixedCode") &&
////               !string.IsNullOrWhiteSpace(fixedOtp);
////    }
////}

//using Microsoft.Extensions.Caching.Memory;
//using Solid.Api.Infrastructure.Sms;

//namespace Solid.Api.Features.Auth;

//public sealed class OtpService(
//    IMemoryCache cache,
//    ISmsSender smsSender,
//    ILogger<OtpService> logger)
//    : IOtpService
//{
//    private const string Prefix = "otp:";

//    public async Task SendRegistrationOtpAsync(string? mobileNumber)
//    {
//        if (string.IsNullOrWhiteSpace(mobileNumber))
//            throw new InvalidOperationException("Mobile number is required");

//        var otp = GenerateOtp();

//        cache.Set($"{Prefix}{mobileNumber}", otp, TimeSpan.FromMinutes(5));

//        logger.LogInformation("OTP for {Mobile}: {Otp}", mobileNumber, otp);

//        await smsSender.SendAsync(
//            mobileNumber,
//            $"Your verification code is: {otp}");
//    }

//    public Task<bool> VerifyRegistrationOtpAsync(string? mobileNumber, string otp)
//    {
//        if (string.IsNullOrWhiteSpace(mobileNumber))
//            return Task.FromResult(false);

//        var key = $"{Prefix}{mobileNumber}";

//        if (!cache.TryGetValue<string>(key, out var storedOtp))
//            return Task.FromResult(false);

//        var isValid = storedOtp == otp;

//        if (isValid)
//            cache.Remove(key);

//        return Task.FromResult(isValid);
//    }

//    public async Task SendPasswordResetOtpAsync(long userId, string? mobileNumber)
//    {
//        if (string.IsNullOrWhiteSpace(mobileNumber))
//            throw new InvalidOperationException("Mobile number is required");

//        var otp = GenerateOtp();

//        cache.Set($"{Prefix}reset:{userId}", otp, TimeSpan.FromMinutes(5));

//        await smsSender.SendAsync(
//            mobileNumber,
//            $"Your reset code is: {otp}");
//    }

//    public Task<bool> VerifyPasswordResetOtpAsync(string userId, string otp)
//    {
//        if (!long.TryParse(userId, out var id))
//            return Task.FromResult(false);

//        var key = $"{Prefix}reset:{id}";

//        if (!cache.TryGetValue<string>(key, out var storedOtp))
//            return Task.FromResult(false);

//        var isValid = storedOtp == otp;

//        if (isValid)
//            cache.Remove(key);

//        return Task.FromResult(isValid);
//    }

//    private static string GenerateOtp()
//        => Random.Shared.Next(100000, 999999).ToString();
//}


using System.Net.Http.Json;

namespace Solid.Api.Infrastructure.Sms;

public sealed class OtpService(
    HttpClient httpClient,
    IConfiguration configuration,
    ILogger<OtpService> logger) : IOtpService
{
    private readonly string _baseUrl = configuration["Sms:Infobip:BaseUrl"]!.TrimEnd('/');
    private readonly string _apiKey = configuration["Sms:Infobip:ApiKey"]!;
    private readonly string _applicationId = configuration["Sms:Infobip:ApplicationId"]!;
    private readonly string _messageId = configuration["Sms:Infobip:MessageId"]!;

    public async Task<string> StartRegistrationOtpAsync(string mobileNumber)
    {
        var url = $"{_baseUrl}/2fa/2/pin";

        var body = new
        {
            applicationId = _applicationId,
            messageId = _messageId,
            from = "SOLID",
            to = mobileNumber
        };

        var result = await SendAsync<StartOtpResponse>(url, body, throwOnError: true);

        if (string.IsNullOrWhiteSpace(result?.PinId))
        {
            logger.LogError("Failed to start OTP for {Mobile}", mobileNumber);
            throw new InvalidOperationException("OTP service failed");
        }

        return result.PinId;
    }

    public async Task<bool> VerifyRegistrationOtpAsync(string pinId, string code)
    {
        var url = $"{_baseUrl}/2fa/2/pin/{pinId}/verify";

        var response = await SendAsync<object>(url, new { pin = code }, throwOnError: false);

        return response != null;
    }

    public Task<string> SendPasswordResetOtpAsync(long userId, string mobileNumber)
        => StartRegistrationOtpAsync(mobileNumber);

    public Task<bool> VerifyPasswordResetOtpAsync(string pinId, string code)
        => VerifyRegistrationOtpAsync(pinId, code);

    private async Task<T?> SendAsync<T>(string url, object body, bool throwOnError)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Content = JsonContent.Create(body);

        var response = await httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Infobip error ({StatusCode}): {Error}", response.StatusCode, content);

            if (throwOnError)
            {
                throw new InvalidOperationException($"Infobip error ({(int)response.StatusCode}): {content}");
            }

            return default;
        }

        return await response.Content.ReadFromJsonAsync<T>();
    }

    private class StartOtpResponse
    {
        public string PinId { get; set; } = default!;
    }
}
