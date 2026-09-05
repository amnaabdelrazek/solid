using Solid.Api.Common;
using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Verify.V2.Service;

namespace Solid.Api.Infrastructure.Sms;

public sealed class OtpService(
    IConfiguration configuration,
    ILogger<OtpService> logger) : IOtpService
{
    private readonly string _accountSid = configuration["Sms:Twilio:AccountSid"] ?? string.Empty;
    private readonly string _authToken = configuration["Sms:Twilio:AuthToken"] ?? string.Empty;
    private readonly string _serviceSid = configuration["Sms:Twilio:VerifyServiceSid"] ?? string.Empty;
    private readonly string _channel = configuration["Sms:Twilio:Channel"] ?? "whatsapp";

    public async Task<string> StartRegistrationOtpAsync(string mobileNumber)
    {
        var normalizedPhoneNumber = NormalizePhoneNumber(mobileNumber);

        if (TryUseFixedCode(out var fixedCode))
        {
            logger.LogWarning("DEV MODE registration OTP for {MobileNumber}: {Otp}", normalizedPhoneNumber, fixedCode);

            return $"local:{normalizedPhoneNumber}";
        }

        EnsureTwilioConfigured();

        try
        {
            TwilioClient.Init(_accountSid, _authToken);

            var verification = await VerificationResource.CreateAsync(
                to: normalizedPhoneNumber,
                channel: _channel,
                pathServiceSid: _serviceSid);

            return verification.Sid;
        }
        catch (ApiException exception)
        {
            throw new InvalidOperationException(
                $"OTP provider rejected the request: {exception.Message}",
                exception);
        }
    }

    public async Task<bool> VerifyRegistrationOtpAsync(string mobileNumber, string code)
    {
        var normalizedPhoneNumber = NormalizePhoneNumber(mobileNumber);

        if (TryUseFixedCode(out var fixedCode))
        {
            return string.Equals(code, fixedCode, StringComparison.Ordinal);
        }

        return await VerifyOtpAsync(normalizedPhoneNumber, code);
    }

    public Task<string> SendPasswordResetOtpAsync(long userId, string mobileNumber)
    {
        return StartRegistrationOtpAsync(mobileNumber);
    }

    public async Task<bool> VerifyPasswordResetOtpAsync(string mobileNumber, string code)
    {
        var normalizedPhoneNumber = NormalizePhoneNumber(mobileNumber);

        if (TryUseFixedCode(out var fixedCode))
        {
            return string.Equals(code, fixedCode, StringComparison.Ordinal);
        }

        return await VerifyOtpAsync(normalizedPhoneNumber, code);
    }

    private async Task<bool> VerifyOtpAsync(string normalizedPhoneNumber, string code)
    {
        EnsureTwilioConfigured();

        try
        {
            TwilioClient.Init(_accountSid, _authToken);

            var result = await VerificationCheckResource.CreateAsync(
                to: normalizedPhoneNumber,
                code: code,
                pathServiceSid: _serviceSid);

            return string.Equals(result.Status, "approved", StringComparison.OrdinalIgnoreCase);
        }
        catch (ApiException exception)
        {
            logger.LogWarning(
                exception,
                "OTP verification failed for {MobileNumber}: {Message}",
                normalizedPhoneNumber,
                exception.Message);

            return false;
        }
    }

    private string NormalizePhoneNumber(string mobileNumber)
    {
        if (!PhoneNumberValidator.TryNormalize(mobileNumber, out var normalizedPhoneNumber) ||
            string.IsNullOrWhiteSpace(normalizedPhoneNumber))
        {
            throw new InvalidOperationException(PhoneNumberValidator.Message);
        }

        return normalizedPhoneNumber;
    }

    private void EnsureTwilioConfigured()
    {
        if (string.IsNullOrWhiteSpace(_accountSid) ||
            string.IsNullOrWhiteSpace(_authToken) ||
            string.IsNullOrWhiteSpace(_serviceSid))
        {
            throw new InvalidOperationException(
                "Twilio Verify is not configured. Set Sms:Twilio:AccountSid, Sms:Twilio:AuthToken, and Sms:Twilio:VerifyServiceSid.");
        }
    }

    private bool TryUseFixedCode(out string fixedCode)
    {
        fixedCode = configuration["Otp:FixedCode"] ?? string.Empty;

        return configuration.GetValue<bool>("Otp:UseFixedCode") &&
               !string.IsNullOrWhiteSpace(fixedCode);
    }
}
