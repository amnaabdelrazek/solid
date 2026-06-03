namespace Solid.Api.Features.Auth;

public interface IOtpService
{
    Task SendRegistrationOtpAsync(long userId, string? mobileNumber);

    Task<bool> VerifyRegistrationOtpAsync(long userId, string otp);

    Task SendPasswordResetOtpAsync(long userId, string? mobileNumber);

    Task<bool> VerifyPasswordResetOtpAsync(string userId, string otp);
}
