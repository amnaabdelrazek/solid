using Microsoft.AspNetCore.Mvc;
using Solid.Api.Common;
using Solid.Api.Features.Shared;
using Solid.Api.Infrastructure.Auth;
using Solid.Api.Database;

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

    private static async Task<IResult> Register([FromBody] RegisterRequest request, IAuthService authService, IDatabase database)
    {
        if (string.IsNullOrWhiteSpace(request.display_name) ||
            string.IsNullOrWhiteSpace(request.mobile_number) ||
            string.IsNullOrWhiteSpace(request.password) ||
            request.substance_ids is null ||
            request.substance_ids.Length == 0)
        {
            return ApiResponse.Fail("The given data was invalid.", StatusCodes.Status422UnprocessableEntity);
        }

        var validationMessage = await ValidateRegistrationReferencesAsync(request, database);
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

        return ApiResponse.Ok(new { user = payload.User, token = payload.Token, token_type = payload.TokenType }, "Register success.");
    }

    private static async Task<string?> ValidateRegistrationReferencesAsync(RegisterRequest request, IDatabase database)
    {
        var lookupCount = await database.ExecuteScalarAsync<int>(
            """
            SELECT COUNT(*)
            FROM lookup_values
            WHERE id IN (@addiction_duration_id, @education_level_id)
            """,
            new { request.addiction_duration_id, request.education_level_id });

        if (lookupCount < 2)
        {
            return "Invalid addiction_duration_id or education_level_id. Use /api/lookups/addiction_duration and /api/lookups/education_level first.";
        }

        var substanceIds = request.substance_ids.Distinct().ToArray();
        var substanceCount = await database.ExecuteScalarAsync<int>(
            """
            SELECT COUNT(*)
            FROM substances
            WHERE id IN (
                SELECT TRY_CAST([value] AS BIGINT)
                FROM STRING_SPLIT(@substanceIds, ',')
            )
            """,
            new { substanceIds = string.Join(',', substanceIds) });

        if (substanceCount < substanceIds.Length)
        {
            return "Invalid substance_ids. Use /api/lookups/substances first.";
        }

        if (request.treatment_type_ids is { Length: > 0 })
        {
            var treatmentTypeIds = request.treatment_type_ids.Distinct().ToArray();
            var treatmentTypeCount = await database.ExecuteScalarAsync<int>(
                """
                SELECT COUNT(*)
                FROM lookup_values
                WHERE id IN (
                    SELECT TRY_CAST([value] AS BIGINT)
                    FROM STRING_SPLIT(@treatmentTypeIds, ',')
                )
                """,
                new { treatmentTypeIds = string.Join(',', treatmentTypeIds) });

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
            return ApiResponse.Fail("Invalid OTP.", StatusCodes.Status422UnprocessableEntity);
        }

        try
        {
            await authService.VerifyAsync(token, request.otp);
        }
        catch
        {
            return ApiResponse.Fail("Invalid OTP.", StatusCodes.Status422UnprocessableEntity);
        }

        return ApiResponse.Ok(message: "Register success.");
    }

    private static async Task<IResult> Login([FromBody] LoginRequest request, IAuthService authService)
    {
        var payload = await authService.LoginAsync(request);
        if (payload is null)
        {
            return ApiResponse.Fail("Unauthenticated.", StatusCodes.Status401Unauthorized);
        }

        return ApiResponse.Ok(new { token = payload.Token, token_type = payload.TokenType, user = payload.User });
    }

    private static async Task<IResult> ForgotPassword([FromBody] ForgotPasswordRequest request, IAuthService authService)
    {
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

        return ApiResponse.Ok(new { token }, "OTP sent.");
    }

    private static async Task<IResult> VerifyForgotOtp([FromBody] VerifyForgotOtpRequest request, IAuthService authService)
    {
        var resetToken = await authService.VerifyForgotOtpAsync(request);
        if (resetToken is null)
        {
            return ApiResponse.Fail("Invalid OTP.", StatusCodes.Status422UnprocessableEntity);
        }

        return ApiResponse.Ok(new { reset_token = resetToken }, "OTP verified.");
    }

    private static async Task<IResult> ResetPassword([FromBody] ResetPasswordRequest request, IAuthService authService)
    {
        if (!await authService.ResetPasswordAsync(request))
        {
            return ApiResponse.Fail("Expired OTP.", StatusCodes.Status422UnprocessableEntity);
        }

        return ApiResponse.Ok(message: "Password reset successfully.");
    }

    private static async Task<IResult> Logout(IAuthContext auth, IDatabase database)
    {
        await database.ExecuteAsync("UPDATE users SET active_device_id = NULL, updated_at = SYSDATETIME() WHERE id = @userId", new { auth.UserId });

        return ApiResponse.Ok(message: "Logged out successfully");
    }

    private static async Task<IResult> Me(IAuthContext auth, IDatabase database)
    {
        var user = await database.UserAsync(auth.UserId);

        return ApiResponse.Ok(new { user = UserResource.From(user!) });
    }

    private static async Task<IResult> DeleteAccount(IAuthContext auth, IAuthService authService)
    {
        await authService.DeleteAccountAsync(auth.UserId);

        return ApiResponse.Ok(message: "Account deleted.");
    }

    private static IResult SmsFailure(InvalidOperationException exception)
    {
        var statusCode = exception.Message.StartsWith("SMS is not configured", StringComparison.OrdinalIgnoreCase)
            ? StatusCodes.Status422UnprocessableEntity
            : StatusCodes.Status502BadGateway;

        return ApiResponse.Fail(exception.Message, statusCode);
    }
}
