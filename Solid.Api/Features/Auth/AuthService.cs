using Microsoft.Extensions.Caching.Memory;
using Solid.Api.Common;
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
    IGroupRepository groupRepository,
    IOtpService otpService,
    IMemoryCache memoryCache,
    IConfiguration configuration) : IAuthService
{
    public async Task<AuthPayload> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await authRepository.FindUserByMobileAsync(request.mobile_number);
        if (existingUser is not null)
        {
            throw new InvalidOperationException("Mobile number already exists.");
        }

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

        await otpService.SendRegistrationOtpAsync(create.MobileNumber);

        var token = Hashing.RandomToken(32);
        memoryCache.Set(PendingRegistrationCacheKey(token), create, PendingRegistrationTtl());

        return new AuthPayload(PendingUserResource(create), token);
    }

    public async Task VerifyAsync(string token, string otp)
    {
        if (!memoryCache.TryGetValue<AuthUserCreate>(PendingRegistrationCacheKey(token), out var create) ||
            create is null)
        {
            throw new InvalidOperationException("Invalid OTP.");
        }

        if (!await otpService.VerifyRegistrationOtpAsync(create.MobileNumber, otp))
        {
            throw new InvalidOperationException("Invalid OTP.");
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        if (await authRepository.FindUserByMobileAsync(create.MobileNumber) is not null)
        {
            throw new InvalidOperationException("Mobile number already exists.");
        }

        var user = await authRepository.CreateInactiveUserAsync(create);
        await authRepository.ActivateUserAsync(user.Id);
        await SubscribeUserToGroupAsync(user.Id);

        await transaction.CommitAsync();
        memoryCache.Remove(PendingRegistrationCacheKey(token));
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

    private static string PendingRegistrationCacheKey(string token)
    {
        return $"pending_registration:{token}";
    }

    private TimeSpan PendingRegistrationTtl()
    {
        var seconds = int.TryParse(configuration["Otp:TtlSeconds"], out var configuredTtl)
            ? configuredTtl
            : 300;

        return TimeSpan.FromSeconds(seconds);
    }

    private static object PendingUserResource(AuthUserCreate create)
    {
        return new
        {
            id = (long?)null,
            display_name = create.DisplayName,
            mobile_number = create.MobileNumber,
            role = "addict",
            preferred_language = create.PreferredLanguage,
            is_active = false
        };
    }

    private async Task SubscribeUserToGroupAsync(long userId)
    {
        if (await groupRepository.HasActiveMembershipAsync(userId))
        {
            return;
        }

        var group = await groupRepository.FindOrCreateForUserSubstanceAsync(userId);
        if (group is null)
        {
            return;
        }

        await groupRepository.AddMemberAsync(group.Id, userId);
    }
}
