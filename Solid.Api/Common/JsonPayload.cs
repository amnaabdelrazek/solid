using System.Text.Json;

namespace Solid.Api.Common;

public static class JsonPayload
{
    public static object? Parse(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<object>(value);
        }
        catch
        {
            return value;
        }
    }

    public static Dictionary<string, object?> ParseObject(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return [];
        }

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, object?>>(value) ?? [];
        }
        catch
        {
            return [];
        }
    }
}
