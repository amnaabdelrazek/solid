namespace Solid.Api.Infrastructure.Sms;

public interface ISmsSender
{
    Task SendOtpAsync(string phoneNumber);
    Task<bool> VerifyOtpAsync(string phoneNumber, string otp);
}
