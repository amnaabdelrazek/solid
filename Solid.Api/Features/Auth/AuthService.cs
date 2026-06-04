using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Solid.Api.Database;
using Solid.Api.Database.Repositories;
using Solid.Api.Features.Shared;
using Solid.Api.Infrastructure.Auth;

namespace Solid.Api.Features.Auth;

public sealed class AuthService(
    SolidDbContext dbContext,
    IAuthRepository authRepository,
    IJwtTokenService jwtTokenService,
    ICacheRepository cacheRepository,
    IOtpService otpService) : IAuthService
{
    public async Task<AuthPayload> RegisterAsync(RegisterRequest request)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        var create = new AuthUserCreate(
            request.display_name,
            request.mobile_number,
            BCrypt.Net.BCrypt.HashPassword(request.password, 12),
            request.preferred_language ?? "ar",
            request.addiction_duration_id,
            request.education_level_id,
            request.had_prior_treatment,
            request.substance_ids,
            request.treatment_type_ids ?? [],
            request.addiction_reason,
            request.days_clean);

        var user = await authRepository.FindUserByMobileAsync(request.mobile_number);
        if (user is null)
        {
            user = await authRepository.CreateInactiveUserAsync(create);
        }
        else
        {
            if (!await authRepository.HasAddictionProfileAsync(user.Id))
            {
                await authRepository.CompleteRegistrationDetailsAsync(user.Id, create);
                user = (await authRepository.FindUserByIdAsync(user.Id))!;
            }
        }

        await otpService.SendRegistrationOtpAsync(user.Id, user.MobileNumber);

        var token = jwtTokenService.Create(user.Id, user.Role, "register");
        await transaction.CommitAsync();

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
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.password, user.Password))
        {
            return null;
        }

        await authRepository.RecordLoginAsync(user.Id, request.device_id);
        var token = jwtTokenService.Create(user.Id, user.Role);

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
        await otpService.SendPasswordResetOtpAsync(user.Id, user.MobileNumber);
        await cacheRepository.PutAsync($"password_reset_token:{token}", Convert.ToString(user.Id), 300);

        return token;
    }

    public async Task<string?> VerifyForgotOtpAsync(VerifyForgotOtpRequest request)
    {
        var userId = await cacheRepository.GetAsync($"password_reset_token:{request.token}");
        if (userId is null || !await otpService.VerifyPasswordResetOtpAsync(userId, request.otp))
        {
            return null;
        }

        var resetToken = Common.Hashing.RandomToken(32);
        await cacheRepository.PutAsync($"password_reset_verified:{resetToken}", userId, 300);

        return resetToken;
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var userId = await cacheRepository.GetAsync($"password_reset_verified:{request.reset_token}");
        if (userId is null || !long.TryParse(userId, out var parsedUserId))
        {
            return false;
        }

        await authRepository.UpdatePasswordAsync(parsedUserId, BCrypt.Net.BCrypt.HashPassword(request.password, 12));

        return true;
    }

    public async Task DeleteAccountAsync(long userId)
    {
        await authRepository.DeactivateAccountAsync(userId);
    }
}
