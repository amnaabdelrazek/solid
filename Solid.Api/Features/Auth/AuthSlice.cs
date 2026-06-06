using Microsoft.AspNetCore.Mvc;
using Solid.Api.Common;
using Solid.Api.Database.Repositories;
using Solid.Api.Features.Shared;
using Solid.Api.Infrastructure.Auth;

namespace Solid.Api.Features.Auth;

public static class AuthSlice
{
    public static IEndpointRouteBuilder MapAuthSlice(this IEndpointRouteBuilder api)
    {
        var auth = api.MapGroup("/auth");

        auth.MapPost("/register", Register);
        auth.MapPost("/verify", Verify);
        auth.MapPost("/login", Login);
        auth.MapPost("/forgot-password", ForgotPassword);
        auth.MapPost("/verify-forgot-otp", VerifyForgotOtp);
        auth.MapPost("/reset-password", ResetPassword);

        var protectedAuth = auth.MapGroup("");
        protectedAuth.RequireLaravelSanctum();
        protectedAuth.MapPost("/logout", Logout);
        protectedAuth.MapGet("/me", Me);
        protectedAuth.MapDelete("/account", DeleteAccount);

        return api;
    }

    private static async Task<IResult> Register([FromBody] RegisterRequest request, IAuthService authService, ILookupRepository lookupRepository)
    {
        if (string.IsNullOrWhiteSpace(request.display_name) ||
            string.IsNullOrWhiteSpace(request.mobile_number) ||
            string.IsNullOrWhiteSpace(request.password) ||
            request.substance_ids is null ||
            request.substance_ids.Length == 0)
        {
            return ApiResponse.Fail("The given data was invalid.", StatusCodes.Status422UnprocessableEntity);
        }

        if (request.display_name.Trim().Length < 2)
            return ApiResponse.Fail("Display name must be at least 2 characters.", StatusCodes.Status422UnprocessableEntity);

        if (request.password.Length < 8)
            return ApiResponse.Fail("Password must be at least 8 characters.", StatusCodes.Status422UnprocessableEntity);

        if (!PhoneNumberValidator.TryNormalize(request.mobile_number, out var normalizedMobileNumber))
        {
            return ApiResponse.Fail(PhoneNumberValidator.Message, StatusCodes.Status422UnprocessableEntity);
        }

        request = request with { mobile_number = normalizedMobileNumber };

        var validationMessage = await ValidateRegistrationReferencesAsync(request, lookupRepository);
        if (validationMessage is not null)
        {
            return ApiResponse.Fail(validationMessage, StatusCodes.Status422UnprocessableEntity);
        }

        AuthPayload payload;
        try
        {
            payload = await authService.RegisterAsync(request);
        }
        catch (InvalidOperationException exception)
        {
            return SmsFailure(exception);
        }

        return ApiResponse.Ok(new { user = payload.User, token = payload.Token, token_type = payload.TokenType }, "Registration successful.");
    }

    private static async Task<string?> ValidateRegistrationReferencesAsync(RegisterRequest request, ILookupRepository lookupRepository)
    {
        var lookupCount = await lookupRepository.CountLookupValuesAsync([request.addiction_duration_id, request.education_level_id]);

        if (lookupCount < 2)
        {
            return "Invalid addiction_duration_id or education_level_id. Use /api/lookups/addiction_duration and /api/lookups/education_level first.";
        }

        var substanceIds = request.substance_ids.Distinct().ToArray();
        var substanceCount = await lookupRepository.CountSubstancesAsync(substanceIds);

        if (substanceCount < substanceIds.Length)
        {
            return "Invalid substance_ids. Use /api/lookups/substances first.";
        }

        if (request.treatment_type_ids is { Length: > 0 })
        {
            var treatmentTypeIds = request.treatment_type_ids.Distinct().ToArray();
            var treatmentTypeCount = await lookupRepository.CountLookupValuesAsync(treatmentTypeIds);

            if (treatmentTypeCount < treatmentTypeIds.Length)
            {
                return "Invalid treatment_type_ids. Use /api/lookups/treatment_type first.";
            }
        }

        return null;
    }

    private static async Task<IResult> Verify(HttpRequest httpRequest, [FromBody] VerifyOtpRequest request, IAuthService authService)
    {
        var token = httpRequest.Headers.Authorization.FirstOrDefault()?.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(token))
        {
            return ApiResponse.Fail("Invalid OTP provided.", StatusCodes.Status422UnprocessableEntity);
        }

        if (string.IsNullOrWhiteSpace(request.otp))
            return ApiResponse.Fail("OTP is required.", StatusCodes.Status422UnprocessableEntity);

        try
        {
            await authService.VerifyAsync(token, request.otp);
        }
        catch
        {
            return ApiResponse.Fail("Invalid OTP provided.", StatusCodes.Status422UnprocessableEntity);
        }

        return ApiResponse.Ok(message: "Registration successful.");
    }

    private static async Task<IResult> Login([FromBody] LoginRequest request, IAuthService authService)
    {
        if (string.IsNullOrWhiteSpace(request.mobile_number) || string.IsNullOrWhiteSpace(request.password))
            return ApiResponse.Fail("Mobile number and password are required.", StatusCodes.Status422UnprocessableEntity);

        if (!PhoneNumberValidator.TryNormalize(request.mobile_number, out var normalizedMobileNumber))
        {
            return ApiResponse.Fail(PhoneNumberValidator.Message, StatusCodes.Status422UnprocessableEntity);
        }

        request = request with { mobile_number = normalizedMobileNumber };

        var payload = await authService.LoginAsync(request);
        if (payload is null)
        {
            return ApiResponse.Fail("Unauthenticated.", StatusCodes.Status401Unauthorized);
        }

        return ApiResponse.Ok(new { token = payload.Token, token_type = payload.TokenType, user = payload.User });
    }

    private static async Task<IResult> ForgotPassword([FromBody] ForgotPasswordRequest request, IAuthService authService)
    {
        if (string.IsNullOrWhiteSpace(request.mobile_number))
            return ApiResponse.Fail("Mobile number is required.", StatusCodes.Status422UnprocessableEntity);

        if (!PhoneNumberValidator.TryNormalize(request.mobile_number, out var normalizedMobileNumber))
        {
            return ApiResponse.Fail(PhoneNumberValidator.Message, StatusCodes.Status422UnprocessableEntity);
        }

        request = request with { mobile_number = normalizedMobileNumber };

        string? token;
        try
        {
            token = await authService.ForgotPasswordAsync(request.mobile_number);
        }
        catch (InvalidOperationException exception)
        {
            return SmsFailure(exception);
        }

        if (token is null)
        {
            return ApiResponse.Fail("Not found.", StatusCodes.Status404NotFound);
        }

        return ApiResponse.Ok(new { token }, "OTP has been sent to your mobile number.");
    }

    private static async Task<IResult> VerifyForgotOtp([FromBody] VerifyForgotOtpRequest request, IAuthService authService)
    {
        if (string.IsNullOrWhiteSpace(request.token) || string.IsNullOrWhiteSpace(request.otp))
            return ApiResponse.Fail("Token and OTP are required.", StatusCodes.Status422UnprocessableEntity);

        var resetToken = await authService.VerifyForgotOtpAsync(request);
        if (resetToken is null)
        {
            return ApiResponse.Fail("Invalid OTP provided.", StatusCodes.Status422UnprocessableEntity);
        }

        return ApiResponse.Ok(new { reset_token = resetToken }, "OTP verified successfully.");
    }

    private static async Task<IResult> ResetPassword([FromBody] ResetPasswordRequest request, IAuthService authService)
    {
        if (string.IsNullOrWhiteSpace(request.reset_token) || string.IsNullOrWhiteSpace(request.password))
            return ApiResponse.Fail("Reset token and password are required.", StatusCodes.Status422UnprocessableEntity);

        if (request.password.Length < 8)
            return ApiResponse.Fail("Password must be at least 8 characters.", StatusCodes.Status422UnprocessableEntity);

        if (!await authService.ResetPasswordAsync(request))
        {
            return ApiResponse.Fail("OTP has expired. Please request a new one.", StatusCodes.Status422UnprocessableEntity);
        }

        return ApiResponse.Ok(message: "Password has been reset successfully.");
    }

    private static async Task<IResult> Logout(
        IAuthContext auth,
        HttpRequest request,
        IAuthRepository authRepository,
        IJwtTokenRevocationService tokenRevocationService)
    {
        var token = request.Headers.Authorization.FirstOrDefault()?.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);

        await authRepository.ClearActiveDeviceAsync(auth.UserId);
        await tokenRevocationService.RevokeAsync(token ?? string.Empty);

        return ApiResponse.Ok(message: "Logged out successfully");
    }

    private static async Task<IResult> Me(IAuthContext auth, IUserRepository userRepository)
    {
        var user = await userRepository.FindAsync(auth.UserId);

        return ApiResponse.Ok(new { user = UserResource.From(user!) });
    }

    private static async Task<IResult> DeleteAccount(IAuthContext auth, IAuthService authService)
    {
        await authService.DeleteAccountAsync(auth.UserId);

        return ApiResponse.Ok(message: "Account deleted successfully.");
    }

    private static IResult SmsFailure(InvalidOperationException exception)
    {
        var statusCode = exception.Message.Contains("phone number", StringComparison.OrdinalIgnoreCase) ||
                         exception.Message.Contains("E.164", StringComparison.OrdinalIgnoreCase) ||
                         exception.Message.Contains("SMS provider rejected", StringComparison.OrdinalIgnoreCase) ||
                         exception.Message.StartsWith("SMS is not configured", StringComparison.OrdinalIgnoreCase)
            ? StatusCodes.Status422UnprocessableEntity
            : StatusCodes.Status502BadGateway;

        return ApiResponse.Fail(exception.Message, statusCode);
    }
}
