using Solid.Api.Database.Entities;

namespace Solid.Api.Features.Shared;

public static class UserFullResource
{
    public static object From(User user)
    {
        var profile = user.AddictionProfile;
        return new
        {
            id = user.Id,
            display_name = user.DisplayName,
            email = user.Email,
            mobile_number = user.MobileNumber,
            username = user.Username,
            role = user.Role,
            bio = user.Bio,
            avatar_url = user.AvatarUrl,
            preferred_language = user.PreferredLanguage,
            is_active = user.IsActive,
            email_verified_at = user.EmailVerifiedAt,
            created_at = user.CreatedAt,
            updated_at = user.UpdatedAt,
            payment_methods = Array.Empty<object>(),
            addiction_profile = profile == null ? null : new
            {
                id = profile.Id,
                addiction_duration_id = profile.AddictionDurationId,
                education_level_id = profile.EducationLevelId,
                had_prior_treatment = profile.HadPriorTreatment,
                addiction_reason = profile.AddictionReason,
                days_clean = profile.DaysClean,
                created_at = profile.CreatedAt,
                updated_at = profile.UpdatedAt
            },
            substances = user.UserSubstances.Select(us => new
            {
                id = us.SubstanceId,
                name_ar = us.Substance?.NameAr,
                name_en = us.Substance?.NameEn
            })
        };
    }
}
