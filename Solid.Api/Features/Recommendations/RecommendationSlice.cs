using Solid.Api.Common;
using Solid.Api.Database;

namespace Solid.Api.Features.Recommendations;

public static class RecommendationSlice
{
    public static IEndpointRouteBuilder MapRecommendationSlice(this IEndpointRouteBuilder api)
    {
        api.MapGet("/recommendations", Index);

        return api;
    }

    private static async Task<IResult> Index(IDatabase database)
    {
        var recommendations = await database.QueryAsync("SELECT * FROM recommendations WHERE is_active = 1 ORDER BY id");

        return ApiResponse.Ok(new { recommendations = recommendations.Select(RecommendationResource.From) });
    }
}
