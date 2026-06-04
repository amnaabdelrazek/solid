using Solid.Api.Common;
using Solid.Api.Database.Repositories;

namespace Solid.Api.Infrastructure.Sms;

public sealed class LocalOtpSender(
    ICacheRepository cacheRepository,
    IConfiguration configuration,
    ILogger<LocalOtpSender> logger) : ISmsSender
{
    public async Task SendOtpAsync(string phoneNumber)
    {
        if (!PhoneNumberValidator.TryNormalize(phoneNumber, out var normalizedPhoneNumber))
        {
            throw new InvalidOperationException(PhoneNumberValidator.Message);
        }

        var otp = configuration["Otp:FixedCode"] ?? configuration["Otp:LocalCode"];
        if (string.IsNullOrWhiteSpace(otp))
        {
            otp = Random.Shared.Next(100000, 999999).ToString();
        }

        var ttlSeconds = int.TryParse(configuration["Otp:TtlSeconds"], out var configuredTtl)
            ? configuredTtl
            : 300;

        await cacheRepository.PutAsync(CacheKey(normalizedPhoneNumber), otp, ttlSeconds);

        logger.LogWarning("LOCAL OTP for {PhoneNumber}: {Otp}", normalizedPhoneNumber, otp);
    }

    public async Task<bool> VerifyOtpAsync(string phoneNumber, string otp)
    {
        if (!PhoneNumberValidator.TryNormalize(phoneNumber, out var normalizedPhoneNumber))
        {
            return false;
        }

        var cachedOtp = await cacheRepository.GetAsync(CacheKey(normalizedPhoneNumber));

        return string.Equals(cachedOtp, otp, StringComparison.Ordinal);
    }

    private static string CacheKey(string phoneNumber)
    {
        return $"otp:{phoneNumber}";
    }
}
