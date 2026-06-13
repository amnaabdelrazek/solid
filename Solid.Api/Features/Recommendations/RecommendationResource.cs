using Solid.Api.Common;
using Solid.Api.Database.Entities;

namespace Solid.Api.Features.Recommendations;

public static class RecommendationResource
{
    public static object From(Recommendation recommendation)
    {
        return new
        {
            id = recommendation.Id,
            created_by_user_id = recommendation.CreatedByUserId,
            substance_category_id = recommendation.SubstanceCategoryId,
            type = recommendation.Type,
            name_ar = recommendation.NameAr,
            name_en = recommendation.NameEn,
            contact_info = recommendation.ContactInfo,
            latitude = recommendation.Latitude,
            longitude = recommendation.Longitude,
            is_active = recommendation.IsActive,
            created_at = EgyptDateTime.Format(recommendation.CreatedAt),
            updated_at = EgyptDateTime.Format(recommendation.UpdatedAt)
        };
    }
}
