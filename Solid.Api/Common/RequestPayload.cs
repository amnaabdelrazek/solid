using System.Globalization;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Primitives;

namespace Solid.Api.Common;

public sealed record RequestPayloadResult<T>(T? Value, IResult? Error);

public static class RequestPayload
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<RequestPayloadResult<T>> ReadAsync<T>(HttpRequest request)
    {
        try
        {
            if (request.HasJsonContentType())
            {
                var value = await request.ReadFromJsonAsync<T>(JsonOptions);

                return value is null
                    ? InvalidBody<T>()
                    : new RequestPayloadResult<T>(value, null);
            }

            if (request.HasFormContentType)
            {
                var form = await request.ReadFormAsync();

                return new RequestPayloadResult<T>(CreateFromForm<T>(form), null);
            }
        }
        catch (Exception exception) when (exception is BadHttpRequestException or JsonException or FormatException or InvalidOperationException)
        {
            return InvalidBody<T>();
        }

        return new RequestPayloadResult<T>(
            default,
            ApiResponse.Fail("Unsupported content type.", StatusCodes.Status415UnsupportedMediaType));
    }

    private static RequestPayloadResult<T> InvalidBody<T>()
    {
        return new RequestPayloadResult<T>(
            default,
            ApiResponse.Fail("Invalid request body.", StatusCodes.Status422UnprocessableEntity));
    }

    private static T CreateFromForm<T>(IFormCollection form)
    {
        var type = typeof(T);
        var constructor = type
            .GetConstructors()
            .OrderByDescending(candidate => candidate.GetParameters().Length)
            .FirstOrDefault()
            ?? throw new InvalidOperationException($"No public constructor found for {type.Name}.");

        var arguments = constructor
            .GetParameters()
            .Select(parameter => FormValue(parameter, form))
            .ToArray();

        return (T)constructor.Invoke(arguments);
    }

    private static object? FormValue(ParameterInfo parameter, IFormCollection form)
    {
        if (!TryGetValues(form, parameter.Name ?? string.Empty, out var values))
        {
            return MissingValue(parameter);
        }

        return ConvertValue(values, parameter.ParameterType);
    }

    private static bool TryGetValues(IFormCollection form, string name, out StringValues values)
    {
        if (form.TryGetValue(name, out values))
        {
            return true;
        }

        if (form.TryGetValue($"{name}[]", out values))
        {
            return true;
        }

        var indexedValues = form
            .Where(field => field.Key.StartsWith($"{name}[", StringComparison.OrdinalIgnoreCase))
            .OrderBy(field => field.Key, StringComparer.OrdinalIgnoreCase)
            .SelectMany(field => field.Value)
            .ToArray();

        values = new StringValues(indexedValues);

        return indexedValues.Length > 0;
    }

    private static object? MissingValue(ParameterInfo parameter)
    {
        if (parameter.HasDefaultValue)
        {
            return parameter.DefaultValue;
        }

        var type = parameter.ParameterType;
        var underlyingType = Nullable.GetUnderlyingType(type);

        if (type.IsArray)
        {
            return Array.CreateInstance(type.GetElementType()!, 0);
        }

        if (underlyingType is not null || !type.IsValueType)
        {
            return type == typeof(string) ? string.Empty : null;
        }

        return Activator.CreateInstance(type);
    }

    private static object? ConvertValue(StringValues values, Type targetType)
    {
        var underlyingType = Nullable.GetUnderlyingType(targetType);
        var actualType = underlyingType ?? targetType;
        var firstValue = values.FirstOrDefault();

        if (underlyingType is not null && string.IsNullOrWhiteSpace(firstValue))
        {
            return null;
        }

        if (actualType.IsArray)
        {
            return ConvertArray(values, actualType.GetElementType()!);
        }

        if (actualType == typeof(string))
        {
            return firstValue ?? string.Empty;
        }

        if (actualType == typeof(object))
        {
            return ConvertObject(firstValue);
        }

        if (string.IsNullOrWhiteSpace(firstValue))
        {
            return Activator.CreateInstance(actualType);
        }

        return ConvertScalar(firstValue, actualType);
    }

    private static Array ConvertArray(StringValues values, Type elementType)
    {
        var items = values
            .SelectMany(value => (value ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .ToArray();

        var array = Array.CreateInstance(elementType, items.Length);

        for (var index = 0; index < items.Length; index++)
        {
            array.SetValue(ConvertScalar(items[index], elementType), index);
        }

        return array;
    }

    private static object? ConvertObject(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<object>(value, JsonOptions);
        }
        catch (JsonException)
        {
            return value;
        }
    }

    private static object ConvertScalar(string value, Type targetType)
    {
        if (targetType == typeof(bool))
        {
            return value.Equals("1", StringComparison.OrdinalIgnoreCase) ||
                   value.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                   value.Equals("on", StringComparison.OrdinalIgnoreCase) ||
                   value.Equals("yes", StringComparison.OrdinalIgnoreCase);
        }

        if (targetType == typeof(DateTime))
        {
            return DateTime.Parse(value, CultureInfo.InvariantCulture);
        }

        if (targetType.IsEnum)
        {
            return Enum.Parse(targetType, value, ignoreCase: true);
        }

        return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
    }
}
