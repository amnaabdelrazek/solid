using Solid.Api.Common;
using Solid.Api.Database.Repositories;
using Solid.Api.Infrastructure.Auth;
using System.Text.Json;

namespace Solid.Api.Features.Content;

public static class ContentSlice
{
    public static IEndpointRouteBuilder MapContentSlice(this IEndpointRouteBuilder api)
    {
        api.MapGet("/privacy-policy", PrivacyPolicy);
        api.MapGet("/terms-and-conditions", TermsAndConditions);
        api.MapPut("/privacy-policy", UpdatePrivacyPolicy).RequireAuthorization();
        api.MapPut("/terms-and-conditions", UpdateTermsAndConditions).RequireAuthorization();

        return api;
    }

    private static async Task<IResult> PrivacyPolicy(ISettingsRepository settingsRepository)
    {
        var value = await settingsRepository.GetAsync("content", "privacy_policy");

        return ApiResponse.Ok(new { privacy_policy = value ?? string.Empty });
    }

    private static async Task<IResult> TermsAndConditions(ISettingsRepository settingsRepository)
    {
        var value = await settingsRepository.GetAsync("content", "terms_and_conditions");

        return ApiResponse.Ok(new { terms_and_conditions = value ?? string.Empty });
    }

    private static async Task<IResult> UpdatePrivacyPolicy(
        IAuthContext auth,
        HttpRequest httpRequest,
        ISettingsRepository settingsRepository)
    {
        var value = await ReadValueAsync(httpRequest);

        if (string.IsNullOrWhiteSpace(value))
            return ApiResponse.Fail("Privacy policy content is required.", StatusCodes.Status422UnprocessableEntity);

        await settingsRepository.SetRawAsync("content", "privacy_policy", value);

        return ApiResponse.Ok(message: "Privacy policy updated.");
    }

    private static async Task<IResult> UpdateTermsAndConditions(
        IAuthContext auth,
        HttpRequest httpRequest,
        ISettingsRepository settingsRepository)
    {
        var value = await ReadValueAsync(httpRequest);

        if (string.IsNullOrWhiteSpace(value))
            return ApiResponse.Fail("Terms and conditions content is required.", StatusCodes.Status422UnprocessableEntity);

        await settingsRepository.SetRawAsync("content", "terms_and_conditions", value);

        return ApiResponse.Ok(message: "Terms and conditions updated.");
    }

    /// <summary>
    /// Reads the request body as either:
    ///   - plain text  (Content-Type: text/plain)  → raw string
    ///   - JSON object { "value": "..." }           → extracts the value field
    ///   - JSON string "..."                        → unwraps the string
    /// </summary>
    private static async Task<string> ReadValueAsync(HttpRequest request)
    {
        using var reader = new StreamReader(request.Body);
        var body = (await reader.ReadToEndAsync()).Trim();

        if (string.IsNullOrWhiteSpace(body))
            return string.Empty;

        try
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            // { "value": "..." }
            if (root.ValueKind == JsonValueKind.Object &&
                root.TryGetProperty("value", out var valueProp))
            {
                return valueProp.ValueKind == JsonValueKind.String
                    ? valueProp.GetString() ?? string.Empty
                    : valueProp.GetRawText();
            }

            // "plain json string"
            if (root.ValueKind == JsonValueKind.String)
                return root.GetString() ?? string.Empty;
        }
        catch
        {
            // Not JSON — treat as raw text (Content-Type: text/plain)
        }

        return body;
    }
}
