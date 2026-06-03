using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Solid.Api.Database.Repositories;
using Solid.Api.Features.Shared;
using Solid.Api.Infrastructure.Auth;

namespace Solid.Api.Features.Auth;

public sealed class AuthService(
    IAuthRepository authRepository,
    IJwtTokenService jwtTokenService,
    Solid.Api.Database.IDatabase database,
    IOtpService otpService) : IAuthService
{
    public async Task<AuthPayload> RegisterAsync(RegisterRequest request)
    {
        var create = new AuthUserCreate(
            request.display_name,
            request.mobile_number,
            BCrypt.Net.BCrypt.HashPassword(request.password, 12),
            request.preferred_language ?? "ar",
            request.addiction_duration_id,
            request.education_level_id,
            request.had_prior_treatment,
            request.substance_ids,
            request.addiction_reason,
            request.days_clean);

        var user = await authRepository.FindUserByMobileAsync(request.mobile_number);
        if (user is null)
        {
            user = await authRepository.CreateInactiveUserAsync(create);
        }
        else
        {
            var userId = Convert.ToInt64(user["id"]!);
            if (!await authRepository.HasAddictionProfileAsync(userId))
            {
                await authRepository.CompleteRegistrationDetailsAsync(userId, create);
                user = (await database.UserAsync(userId))!;
            }
        }

        await otpService.SendRegistrationOtpAsync(Convert.ToInt64(user["id"]!), Convert.ToString(user["mobile_number"]));

        var token = jwtTokenService.Create(Convert.ToInt64(user["id"]!), Convert.ToString(user["role"]), "register");

        return new AuthPayload(UserResource.From(user), token);
    }

    public async Task VerifyAsync(string token, string otp)
    {
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        var userId = Convert.ToInt64(jwt.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier || claim.Type == "nameid").Value);

        if (!await otpService.VerifyRegistrationOtpAsync(userId, otp))
        {
            throw new InvalidOperationException("Invalid OTP.");
        }

        await authRepository.ActivateUserAsync(userId);
    }

    public async Task<AuthPayload?> LoginAsync(LoginRequest request)
    {
        var user = await authRepository.FindUserByMobileAsync(request.mobile_number, onlyActive: true);
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.password, Convert.ToString(user["password"])))
        {
            return null;
        }

        await authRepository.RecordLoginAsync(Convert.ToInt64(user["id"]!), request.device_id);
        var token = jwtTokenService.Create(Convert.ToInt64(user["id"]!), Convert.ToString(user["role"]));

        return new AuthPayload(UserResource.From(user), token);
    }

    public async Task<string?> ForgotPasswordAsync(string mobileNumber)
    {
        var user = await authRepository.FindUserByMobileAsync(mobileNumber);
        if (user is null)
        {
            return null;
        }

        var token = Common.Hashing.RandomToken(32);
        await database.PutCacheAsync($"password_reset_token:{token}", Convert.ToString(user["id"])!, 300);
        await otpService.SendPasswordResetOtpAsync(Convert.ToInt64(user["id"]!), Convert.ToString(user["mobile_number"]));

        return token;
    }

    public async Task<string?> VerifyForgotOtpAsync(VerifyForgotOtpRequest request)
    {
        var userId = await database.GetCacheAsync($"password_reset_token:{request.token}");
        if (userId is null || !await otpService.VerifyPasswordResetOtpAsync(userId, request.otp))
        {
            return null;
        }

        var resetToken = Common.Hashing.RandomToken(32);
        await database.PutCacheAsync($"password_reset_verified:{resetToken}", userId, 300);

        return resetToken;
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var userId = await database.GetCacheAsync($"password_reset_verified:{request.reset_token}");
        if (userId is null)
        {
            return false;
        }

        await database.ExecuteAsync("UPDATE users SET [password] = @password, updated_at = SYSDATETIME() WHERE id = @userId", new { password = BCrypt.Net.BCrypt.HashPassword(request.password, 12), userId });

        return true;
    }

    public async Task DeleteAccountAsync(long userId)
    {
        await authRepository.DeactivateAccountAsync(userId);
    }
}
