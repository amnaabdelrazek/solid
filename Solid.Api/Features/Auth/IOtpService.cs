//namespace Solid.Api.Features.Auth;

//public interface IOtpService
//{
//    //Task SendRegistrationOtpAsync(string? mobileNumber);

//    //Task<bool> VerifyRegistrationOtpAsync(string? mobileNumber, string otp);

//    //Task SendPasswordResetOtpAsync(long userId, string? mobileNumber);

//    //Task<bool> VerifyPasswordResetOtpAsync(string userId, string otp);

//    Task SendRegistrationOtpAsync(string? mobileNumber);
//    Task<bool> VerifyRegistrationOtpAsync(string? mobileNumber, string otp);

//    Task SendPasswordResetOtpAsync(long userId, string? mobileNumber);
//    Task<bool> VerifyPasswordResetOtpAsync(string userId, string otp);
//}

namespace Solid.Api.Infrastructure.Sms;

public interface IOtpService
{
    Task<string> StartRegistrationOtpAsync(string mobileNumber);

    Task<bool> VerifyRegistrationOtpAsync(string pinId, string code);

    Task<string> SendPasswordResetOtpAsync(long userId, string mobileNumber);

    Task<bool> VerifyPasswordResetOtpAsync(string pinId, string code);
}
