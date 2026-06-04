using Solid.Api.Common;
using Solid.Api.Database.Repositories;

namespace Solid.Api.Features.Content;

public static class ContentSlice
{
    public static IEndpointRouteBuilder MapContentSlice(this IEndpointRouteBuilder api)
    {
        api.MapGet("/privacy-policy", PrivacyPolicy);
        api.MapGet("/terms-and-conditions", TermsAndConditions);

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
}
