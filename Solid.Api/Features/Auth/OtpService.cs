using Solid.Api.Database.Repositories;
using Solid.Api.Infrastructure.Sms;

namespace Solid.Api.Features.Auth;

public sealed class OtpService(
    IAuthRepository authRepository,
    ISmsSender smsSender,
    IConfiguration configuration,
    ILogger<OtpService> logger)
    : IOtpService
{
    public async Task SendRegistrationOtpAsync(
        string? mobileNumber)
    {
        if (string.IsNullOrWhiteSpace(mobileNumber))
        {
            throw new InvalidOperationException(
                "User has no mobile number.");
        }

        if (TryUseFixedOtp(out var fixedOtp))
        {
            logger.LogWarning("DEV FIXED OTP for registration mobile {MobileNumber}: {Otp}", mobileNumber, fixedOtp);

            return;
        }

        await smsSender.SendOtpAsync(mobileNumber);
    }

    public async Task<bool> VerifyRegistrationOtpAsync(
        string? mobileNumber,
        string otp)
    {
        if (string.IsNullOrWhiteSpace(mobileNumber))
        {
            return false;
        }

        if (TryUseFixedOtp(out var fixedOtp))
        {
            return string.Equals(otp, fixedOtp, StringComparison.Ordinal);
        }

        return await smsSender.VerifyOtpAsync(
            mobileNumber,
            otp);
    }

    public async Task SendPasswordResetOtpAsync(
        long userId,
        string? mobileNumber)
    {
        if (string.IsNullOrWhiteSpace(mobileNumber))
        {
            throw new InvalidOperationException(
                "User has no mobile number.");
        }

        if (TryUseFixedOtp(out var fixedOtp))
        {
            logger.LogWarning("DEV FIXED OTP for password reset user {UserId}: {Otp}", userId, fixedOtp);

            return;
        }

        await smsSender.SendOtpAsync(mobileNumber);
    }

    public async Task<bool> VerifyPasswordResetOtpAsync(
        string userId,
        string otp)
    {
        if (!long.TryParse(userId, out var id))
        {
            return false;
        }

        var user = await authRepository.FindUserByIdAsync(id);

        if (user is null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(user.MobileNumber))
        {
            return false;
        }

        if (TryUseFixedOtp(out var fixedOtp))
        {
            return string.Equals(otp, fixedOtp, StringComparison.Ordinal);
        }

        return await smsSender.VerifyOtpAsync(
            user.MobileNumber,
            otp);
    }

    private bool TryUseFixedOtp(out string fixedOtp)
    {
        fixedOtp = configuration["Otp:FixedCode"] ?? string.Empty;

        return configuration.GetValue<bool>("Otp:UseFixedCode") &&
               !string.IsNullOrWhiteSpace(fixedOtp);
    }
}
