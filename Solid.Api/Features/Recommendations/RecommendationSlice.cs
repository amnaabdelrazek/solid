using Microsoft.AspNetCore.Mvc;
using Solid.Api.Common;
using Solid.Api.Database.Repositories;
using Solid.Api.Infrastructure.Auth;

namespace Solid.Api.Features.Recommendations;

public static class RecommendationSlice
{
    public static IEndpointRouteBuilder MapRecommendationSlice(this IEndpointRouteBuilder api)
    {
        api.MapGet("/recommendations", Index);
        api.MapPost("/recommendations", Create);
        api.MapPut("/recommendations/{id:long}", Update);
        api.MapDelete("/recommendations/{id:long}", Delete);

        return api;
    }

    // GET /api/recommendations - list all active recommendations
    private static async Task<IResult> Index(IRecommendationRepository recommendationRepository)
    {
        var recommendations = await recommendationRepository.ActiveAsync();
        return ApiResponse.Ok(new { recommendations = recommendations.Select(RecommendationResource.From) });
    }

    // POST /api/recommendations - user creates recommendation (must be member of the session)
    private static async Task<IResult> Create(
        IAuthContext auth,
        [FromBody] CreateRecommendationRequest request,
        IRecommendationRepository recommendationRepository,
        ISessionRepository sessionRepository)
    {
        if (string.IsNullOrWhiteSpace(request.name_ar))
            return ApiResponse.Fail("name_ar is required.", StatusCodes.Status422UnprocessableEntity);

        if (string.IsNullOrWhiteSpace(request.type))
            return ApiResponse.Fail("type is required.", StatusCodes.Status422UnprocessableEntity);

        // If session_id provided, verify user was in that session
        if (request.session_id.HasValue)
        {
            var session = await sessionRepository.FindAsync(request.session_id.Value, auth.UserId, auth.Role);
            if (session is null)
                return ApiResponse.Fail("Session not found or you are not a member.", StatusCodes.Status403Forbidden);
        }

        // Validate coordinates if provided
        if (request.latitude.HasValue && (request.latitude < -90 || request.latitude > 90))
            return ApiResponse.Fail("Invalid latitude value.", StatusCodes.Status422UnprocessableEntity);
        if (request.longitude.HasValue && (request.longitude < -180 || request.longitude > 180))
            return ApiResponse.Fail("Invalid longitude value.", StatusCodes.Status422UnprocessableEntity);

        var recommendation = await recommendationRepository.CreateAsync(auth.UserId, new RecommendationCreate(
            request.substance_category_id,
            request.type,
            request.name_ar,
            request.name_en,
            request.contact_info,
            request.latitude,
            request.longitude));

        return ApiResponse.Ok(new { recommendation = RecommendationResource.From(recommendation) }, "Recommendation created successfully.");
    }

    // PUT /api/recommendations/{id} - user updates their own recommendation
    private static async Task<IResult> Update(
        long id,
        IAuthContext auth,
        [FromBody] UpdateRecommendationRequest request,
        IRecommendationRepository recommendationRepository)
    {
        if (request.latitude.HasValue && (request.latitude < -90 || request.latitude > 90))
            return ApiResponse.Fail("Invalid latitude value.", StatusCodes.Status422UnprocessableEntity);
        if (request.longitude.HasValue && (request.longitude < -180 || request.longitude > 180))
            return ApiResponse.Fail("Invalid longitude value.", StatusCodes.Status422UnprocessableEntity);

        var recommendation = await recommendationRepository.FindUserRecommendationAsync(id, auth.UserId);
        if (recommendation is null)
            return ApiResponse.Fail("Recommendation not found or you do not own it.", StatusCodes.Status404NotFound);

        var updated = await recommendationRepository.UpdateAsync(id, auth.UserId, new RecommendationUpdate(
            request.type,
            request.name_ar,
            request.name_en,
            request.contact_info,
            request.latitude,
            request.longitude,
            request.is_active));

        return ApiResponse.Ok(new { recommendation = RecommendationResource.From(updated!) }, "Recommendation updated successfully.");
    }

    // DELETE /api/recommendations/{id}
    private static async Task<IResult> Delete(
        long id,
        IAuthContext auth,
        IRecommendationRepository recommendationRepository)
    {
        var recommendation = await recommendationRepository.FindUserRecommendationAsync(id, auth.UserId);
        if (recommendation is null)
            return ApiResponse.Fail("Recommendation not found or you do not own it.", StatusCodes.Status404NotFound);

        await recommendationRepository.DeactivateAsync(id, auth.UserId);
        return ApiResponse.Ok(message: "Recommendation removed.");
    }
}

public sealed record CreateRecommendationRequest(
    long? substance_category_id,
    string type,
    string name_ar,
    string? name_en,
    string? contact_info,
    decimal? latitude,
    decimal? longitude,
    long? session_id);

public sealed record UpdateRecommendationRequest(
    string? type,
    string? name_ar,
    string? name_en,
    string? contact_info,
    decimal? latitude,
    decimal? longitude,
    bool? is_active);
