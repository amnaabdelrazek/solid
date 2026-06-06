using Microsoft.AspNetCore.Mvc;
using Solid.Api.Common;
using Solid.Api.Database.Repositories;
using Solid.Api.Features.Settings;
using Solid.Api.Infrastructure.Auth;

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
        [FromBody] SettingRequest request,
        ISettingsRepository settingsRepository)
    {
        //if (!auth.IsAdminOrInstructor())
        //{
        //    return ApiResponse.Fail("This action is unauthorized.", StatusCodes.Status403Forbidden);
        //}

        await settingsRepository.SetAsync("content", "privacy_policy", request.value);

        return ApiResponse.Ok(message: "Privacy policy updated.");
    }

    private static async Task<IResult> UpdateTermsAndConditions(
        IAuthContext auth,
        [FromBody] SettingRequest request,
        ISettingsRepository settingsRepository)
    {
        //if (!auth.IsAdminOrInstructor())
        //{
        //    return ApiResponse.Fail("This action is unauthorized.", StatusCodes.Status403Forbidden);
        //}

        await settingsRepository.SetAsync("content", "terms_and_conditions", request.value);

        return ApiResponse.Ok(message: "Terms updated.");
    }
}
