namespace Solid.Api.Features.Auth;

public interface IOtpService
{
    Task SendRegistrationOtpAsync(string? mobileNumber);

    Task<bool> VerifyRegistrationOtpAsync(string? mobileNumber, string otp);

    Task SendPasswordResetOtpAsync(long userId, string? mobileNumber);

    Task<bool> VerifyPasswordResetOtpAsync(string userId, string otp);
}
