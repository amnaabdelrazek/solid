namespace Solid.Api.Features.Recommendations;

public static class RecommendationResource
{
    public static object From(Dictionary<string, object?> recommendation)
    {
        return new
        {
            id = recommendation["id"],
            substance_category_id = recommendation.GetValueOrDefault("substance_category_id"),
            type = recommendation.GetValueOrDefault("type"),
            name_ar = recommendation.GetValueOrDefault("name_ar"),
            name_en = recommendation.GetValueOrDefault("name_en"),
            contact_info = recommendation.GetValueOrDefault("contact_info"),
            latitude = recommendation.GetValueOrDefault("latitude"),
            longitude = recommendation.GetValueOrDefault("longitude"),
            is_active = recommendation.GetValueOrDefault("is_active"),
            created_at = recommendation.GetValueOrDefault("created_at"),
            updated_at = recommendation.GetValueOrDefault("updated_at")
        };
    }
}
