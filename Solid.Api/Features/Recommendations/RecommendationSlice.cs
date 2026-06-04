using Solid.Api.Common;
using Solid.Api.Database.Repositories;

namespace Solid.Api.Features.Recommendations;

public static class RecommendationSlice
{
    public static IEndpointRouteBuilder MapRecommendationSlice(this IEndpointRouteBuilder api)
    {
        api.MapGet("/recommendations", Index);

        return api;
    }

    private static async Task<IResult> Index(IRecommendationRepository recommendationRepository)
    {
        var recommendations = await recommendationRepository.ActiveAsync();

        return ApiResponse.Ok(new { recommendations = recommendations.Select(RecommendationResource.From) });
    }
}
