using System.Security.Cryptography;
using Solid.Api.Database;
using Solid.Api.Features.Shared;
using Solid.Api.Infrastructure.Sms;

namespace Solid.Api.Features.Auth;

public sealed class OtpService(IDatabase database, ISmsSender smsSender, IConfiguration configuration) : IOtpService
{
    public async Task SendRegistrationOtpAsync(long userId, string? mobileNumber)
    {
        await SendOtpAsync($"otp:{userId}", mobileNumber);
    }

    public async Task<bool> VerifyRegistrationOtpAsync(long userId, string otp)
    {
        return await VerifyOtpAsync($"otp:{userId}", otp);
    }

    public async Task SendPasswordResetOtpAsync(long userId, string? mobileNumber)
    {
        await SendOtpAsync($"password_reset_otp:{userId}", mobileNumber);
    }

    public async Task<bool> VerifyPasswordResetOtpAsync(string userId, string otp)
    {
        return await VerifyOtpAsync($"password_reset_otp:{userId}", otp);
    }

    private async Task SendOtpAsync(string cacheKey, string? mobileNumber)
    {
        if (string.IsNullOrWhiteSpace(mobileNumber))
        {
            throw new InvalidOperationException("User has no mobile number.");
        }

        var otp = GenerateOtp();
        var ttlSeconds = int.TryParse(configuration["Otp:TtlSeconds"], out var configuredTtl)
            ? configuredTtl
            : 300;

        await database.PutCacheAsync(cacheKey, otp, ttlSeconds);
        await smsSender.SendAsync(mobileNumber, $"Your OTP code is: {otp}");
    }

    private async Task<bool> VerifyOtpAsync(string cacheKey, string otp)
    {
        var storedOtp = await database.GetCacheAsync(cacheKey);

        return !string.IsNullOrWhiteSpace(storedOtp) && storedOtp == otp;
    }

    private static string GenerateOtp()
    {
        return RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
    }
}
