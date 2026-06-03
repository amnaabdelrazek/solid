namespace Solid.Api.Features.Auth;

public sealed record RegisterRequest(
    string display_name,
    string mobile_number,
    string password,
    string? preferred_language,
    long addiction_duration_id,
    long education_level_id,
    bool had_prior_treatment,
    long[] substance_ids,
    long[]? treatment_type_ids,
    string? addiction_reason,
    int? days_clean);

public sealed record LoginRequest(string mobile_number, string password, string device_id);

public sealed record VerifyOtpRequest(string otp);

public sealed record ForgotPasswordRequest(string mobile_number);

public sealed record VerifyForgotOtpRequest(string token, string otp);

public sealed record ResetPasswordRequest(string reset_token, string password);
