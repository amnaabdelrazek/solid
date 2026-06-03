using Solid.Api.Common;
using Solid.Api.Features.Shared;
using Solid.Api.Database;

namespace Solid.Api.Features.Content;

public static class ContentSlice
{
    public static IEndpointRouteBuilder MapContentSlice(this IEndpointRouteBuilder api)
    {
        api.MapGet("/privacy-policy", PrivacyPolicy);
        api.MapGet("/terms-and-conditions", TermsAndConditions);

        return api;
    }

    private static async Task<IResult> PrivacyPolicy(IDatabase database)
    {
        var value = await database.SettingAsync("content", "privacy_policy");

        return ApiResponse.Ok(new { privacy_policy = value ?? string.Empty });
    }

    private static async Task<IResult> TermsAndConditions(IDatabase database)
    {
        var value = await database.SettingAsync("content", "terms_and_conditions");

        return ApiResponse.Ok(new { terms_and_conditions = value ?? string.Empty });
    }
}
