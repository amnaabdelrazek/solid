using System.Globalization;

namespace Solid.Api.Common;

public static class EgyptDateTime
{
    private const string FullFormat = "dd/MM/yyyy HH:mm:ss";
    private const string DateFormat = "dd/MM/yyyy";
    private const string TimeFormat = "HH:mm:ss";

    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

    private static readonly TimeZoneInfo TimeZone = ResolveTimeZone();

    public static string? Format(DateTime? value)
    {
        return value.HasValue ? Format(value.Value) : null;
    }

    public static string Format(DateTime value)
    {
        return ToEgyptTime(value).ToString(FullFormat, Culture);
    }

    public static string Date(DateTime value)
    {
        return ToEgyptTime(value).ToString(DateFormat, Culture);
    }

    public static string Time(DateTime value)
    {
        return ToEgyptTime(value).ToString(TimeFormat, Culture);
    }

    public static DateTime ToUtcFromEgyptClock(DateTime value)
    {
        var egyptClock = DateTime.SpecifyKind(value, DateTimeKind.Unspecified);

        return TimeZoneInfo.ConvertTimeToUtc(egyptClock, TimeZone);
    }

    private static DateTime ToEgyptTime(DateTime value)
    {
        var utcValue = value.Kind == DateTimeKind.Utc
            ? value
            : DateTime.SpecifyKind(value, DateTimeKind.Utc);

        return TimeZoneInfo.ConvertTimeFromUtc(utcValue, TimeZone);
    }

    private static TimeZoneInfo ResolveTimeZone()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo");
        }
    }
}
