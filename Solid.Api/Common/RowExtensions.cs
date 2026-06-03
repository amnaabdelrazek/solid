using System.Text.Json;

namespace Solid.Api.Common;

public static class RowExtensions
{
    public static T? Value<T>(this IReadOnlyDictionary<string, object?> row, string key)
    {
        if (!row.TryGetValue(key, out var value) || value is null || value is DBNull)
        {
            return default;
        }

        return (T)Convert.ChangeType(value, Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T));
    }

    public static string Text(this IReadOnlyDictionary<string, object?> row, string key)
    {
        return row.TryGetValue(key, out var value) ? Convert.ToString(value) ?? string.Empty : string.Empty;
    }

    public static object? JsonValue(this IReadOnlyDictionary<string, object?> row, string key)
    {
        if (!row.TryGetValue(key, out var value) || value is null || value is DBNull)
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, object?>>(Convert.ToString(value)!);
        }
        catch
        {
            return value;
        }
    }

    public static string Translated(this IReadOnlyDictionary<string, object?> row, string key, string locale)
    {
        return row.Text($"{key}_{locale}") is { Length: > 0 } translated
            ? translated
            : row.Text($"{key}_ar");
    }
}
