using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Verify.V2.Service;
using Solid.Api.Common;

namespace Solid.Api.Infrastructure.Sms;

public sealed class TwilioVerifySender : ISmsSender
{
    private readonly IConfiguration _configuration;

    public TwilioVerifySender(IConfiguration configuration)
    {
        _configuration = configuration;

        var accountSid = _configuration["Sms:Twilio:AccountSid"];
        var authToken = _configuration["Sms:Twilio:AuthToken"];

        if (string.IsNullOrWhiteSpace(accountSid) ||
            string.IsNullOrWhiteSpace(authToken))
        {
            throw new InvalidOperationException(
                "Twilio Verify is not configured.");
        }

        TwilioClient.Init(accountSid, authToken);
    }

    public async Task SendOtpAsync(string phoneNumber)
    {
        if (!PhoneNumberValidator.TryNormalize(phoneNumber, out var normalizedPhoneNumber))
        {
            throw new InvalidOperationException(PhoneNumberValidator.Message);
        }

        var serviceSid =
            _configuration["Sms:Twilio:VerifyServiceSid"];

        if (string.IsNullOrWhiteSpace(serviceSid))
        {
            throw new InvalidOperationException(
                "VerifyServiceSid is missing.");
        }

        try
        {
            await VerificationResource.CreateAsync(
                to: normalizedPhoneNumber,
                channel: "sms",
                pathServiceSid: serviceSid
            );
        }
        catch (ApiException exception)
        {
            throw new InvalidOperationException(
                $"SMS provider rejected the request: {exception.Message}",
                exception);
        }
    }

    public async Task<bool> VerifyOtpAsync(
        string phoneNumber,
        string otp)
    {
        if (!PhoneNumberValidator.TryNormalize(phoneNumber, out var normalizedPhoneNumber))
        {
            return false;
        }

        var serviceSid =
            _configuration["Sms:Twilio:VerifyServiceSid"];

        try
        {
            var result =
                await VerificationCheckResource.CreateAsync(
                    to: normalizedPhoneNumber,
                    code: otp,
                    pathServiceSid: serviceSid
                );

            return result.Status == "approved";
        }
        catch (ApiException)
        {
            return false;
        }
    }
}
