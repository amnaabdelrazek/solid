using System.Text.RegularExpressions;

namespace Solid.Api.Common;

public static partial class PhoneNumberValidator
{
    public const string Message = "Mobile number must be a valid Egyptian mobile number, e.g. 01099344359 or +201099344359.";

    public static bool IsE164(string? phoneNumber)
    {
        return !string.IsNullOrWhiteSpace(phoneNumber) && E164Regex().IsMatch(phoneNumber);
    }

    public static bool TryNormalize(string? phoneNumber, out string normalizedPhoneNumber)
    {
        normalizedPhoneNumber = string.Empty;

        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return false;
        }

        var cleanedPhoneNumber = phoneNumber
            .Trim()
            .Replace(" ", string.Empty)
            .Replace("-", string.Empty)
            .Replace("(", string.Empty)
            .Replace(")", string.Empty);

        if (E164Regex().IsMatch(cleanedPhoneNumber))
        {
            normalizedPhoneNumber = cleanedPhoneNumber;

            return true;
        }

        if (EgyptLocalRegex().IsMatch(cleanedPhoneNumber))
        {
            normalizedPhoneNumber = $"+2{cleanedPhoneNumber}";

            return true;
        }

        if (EgyptInternationalWithoutPlusRegex().IsMatch(cleanedPhoneNumber))
        {
            normalizedPhoneNumber = $"+{cleanedPhoneNumber}";

            return true;
        }

        if (EgyptInternationalWithZerosRegex().IsMatch(cleanedPhoneNumber))
        {
            normalizedPhoneNumber = $"+{cleanedPhoneNumber[2..]}";

            return true;
        }

        return false;
    }

    [GeneratedRegex("^\\+[1-9]\\d{7,14}$")]
    private static partial Regex E164Regex();

    [GeneratedRegex("^01[0125]\\d{8}$")]
    private static partial Regex EgyptLocalRegex();

    [GeneratedRegex("^201[0125]\\d{8}$")]
    private static partial Regex EgyptInternationalWithoutPlusRegex();

    [GeneratedRegex("^00201[0125]\\d{8}$")]
    private static partial Regex EgyptInternationalWithZerosRegex();
}
