using Microsoft.Extensions.Caching.Memory;
using Solid.Api.Common;
using Solid.Api.Database;
using Solid.Api.Database.Repositories;
using Solid.Api.Features.Shared;
using Solid.Api.Infrastructure.Auth;
using Solid.Api.Infrastructure.Sms;

namespace Solid.Api.Features.Auth;

public sealed class AuthService(
    IAuthRepository authRepository,
    IJwtTokenService jwtTokenService,
    ICacheRepository cacheRepository,
    IGroupRepository groupRepository,
    IOtpService otpService,
    IMemoryCache memoryCache,
    IConfiguration configuration,
    SolidDbContext dbContext) : IAuthService
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

        // Registration OTP is disabled; new users are created and activated immediately.
        // await otpService.StartRegistrationOtpAsync(create.MobileNumber);

        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        var user = await authRepository.CreateInactiveUserAsync(create);
        await authRepository.ActivateUserAsync(user.Id);
        await SubscribeUserToGroupAsync(user.Id);

        await transaction.CommitAsync();

        var activeUser = await authRepository.FindUserByIdAsync(user.Id) ?? user;
        var token = jwtTokenService.Create(activeUser.Id, activeUser.Role);

        return new AuthPayload(UserResource.From(activeUser), token);
    }

    public async Task<AuthPayload> VerifyAsync(string token, string otp)
    {
        if (!memoryCache.TryGetValue<AuthUserCreate>(
                PendingRegistrationCacheKey(token),
                out var create) || create is null)
        {
            throw new InvalidOperationException("Invalid or expired OTP.");
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

        var activeUser = await authRepository.FindUserByIdAsync(user.Id) ?? user;
        var jwt = jwtTokenService.Create(activeUser.Id, activeUser.Role);

        return new AuthPayload(UserResource.From(activeUser), jwt);
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

        if (string.IsNullOrWhiteSpace(user.MobileNumber))
        {
            throw new InvalidOperationException("User has no mobile number.");
        }

        await otpService.SendPasswordResetOtpAsync(user.Id, user.MobileNumber);

        var token = Hashing.RandomToken(32);
        await cacheRepository.PutAsync($"password_reset_token:{token}", user.Id.ToString(), OtpTtlSeconds());

        return token;
    }

    public async Task<string?> VerifyForgotOtpAsync(VerifyForgotOtpRequest request)
    {
        var userId = await cacheRepository.GetAsync($"password_reset_token:{request.token}");

        if (userId is null || !long.TryParse(userId, out var parsedUserId))
        {
            return null;
        }

        var user = await authRepository.FindUserByIdAsync(parsedUserId);
        if (string.IsNullOrWhiteSpace(user?.MobileNumber))
        {
            return null;
        }

        if (!await otpService.VerifyPasswordResetOtpAsync(user.MobileNumber, request.otp))
        {
            return null;
        }

        var resetToken = Hashing.RandomToken(32);
        await cacheRepository.PutAsync($"password_reset_verified:{resetToken}", userId, OtpTtlSeconds());

        return resetToken;
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var userId = await cacheRepository.GetAsync($"password_reset_verified:{request.reset_token}");

        if (userId is null || !long.TryParse(userId, out var id))
        {
            return false;
        }

        await authRepository.UpdatePasswordAsync(
            id,
            BCrypt.Net.BCrypt.HashPassword(request.password, 12));

        return true;
    }

    public async Task DeleteAccountAsync(long userId)
    {
        await authRepository.DeactivateAccountAsync(userId);
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

    private static string PendingRegistrationCacheKey(string token)
    {
        return $"pending_registration:{token}";
    }

    private TimeSpan PendingRegistrationTtl()
    {
        return TimeSpan.FromSeconds(OtpTtlSeconds());
    }

    private int OtpTtlSeconds()
    {
        return int.TryParse(configuration["Otp:TtlSeconds"], out var seconds)
            ? seconds
            : 300;
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
}
