namespace Solid.Api.Infrastructure.Sms;

public interface IOtpService
{
    Task<string> StartRegistrationOtpAsync(string mobileNumber);

    Task<bool> VerifyRegistrationOtpAsync(string mobileNumber, string code);

    Task<string> SendPasswordResetOtpAsync(long userId, string mobileNumber);

    Task<bool> VerifyPasswordResetOtpAsync(string mobileNumber, string code);
}
