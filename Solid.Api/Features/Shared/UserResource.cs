using Solid.Api.Common;
using Solid.Api.Database.Entities;

namespace Solid.Api.Features.Shared;

public static class UserResource
{
    public static object From(User user)
    {
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
            email_verified_at = EgyptDateTime.Format(user.EmailVerifiedAt),
            created_at = EgyptDateTime.Format(user.CreatedAt),
            updated_at = EgyptDateTime.Format(user.UpdatedAt),
            payment_methods = Array.Empty<object>()
        };
    }
}
