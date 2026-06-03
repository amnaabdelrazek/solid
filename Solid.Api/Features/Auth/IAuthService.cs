namespace Solid.Api.Features.Auth;

public interface IAuthService
{
    Task<AuthPayload> RegisterAsync(RegisterRequest request);

    Task VerifyAsync(string token, string otp);

    Task<AuthPayload?> LoginAsync(LoginRequest request);

    Task<string?> ForgotPasswordAsync(string mobileNumber);

    Task<string?> VerifyForgotOtpAsync(VerifyForgotOtpRequest request);

    Task<bool> ResetPasswordAsync(ResetPasswordRequest request);

    Task DeleteAccountAsync(long userId);
}

public sealed record AuthPayload(object User, string Token, string TokenType = "Bearer");
