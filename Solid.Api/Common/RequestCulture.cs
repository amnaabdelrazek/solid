namespace Solid.Api.Common;

public static class RequestCulture
{
    public static string Locale(HttpRequest request)
    {
        return request.Headers.AcceptLanguage.FirstOrDefault()?.StartsWith("en", StringComparison.OrdinalIgnoreCase) == true
            ? "en"
            : "ar";
    }
}
