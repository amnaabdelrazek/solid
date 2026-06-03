using Solid.Api.Common;

namespace Solid.Api.Features.Shared;

public static class UserResource
{
    public static object From(Dictionary<string, object?> user)
    {
        return new
        {
            id = user["id"],
            display_name = user.GetValueOrDefault("display_name"),
            email = user.GetValueOrDefault("email"),
            mobile_number = user.GetValueOrDefault("mobile_number"),
            username = user.GetValueOrDefault("username"),
            role = user.GetValueOrDefault("role"),
            bio = user.GetValueOrDefault("bio"),
            avatar_url = user.GetValueOrDefault("avatar_url"),
            preferred_language = user.GetValueOrDefault("preferred_language"),
            is_active = user.GetValueOrDefault("is_active"),
            email_verified_at = user.GetValueOrDefault("email_verified_at"),
            created_at = user.GetValueOrDefault("created_at"),
            updated_at = user.GetValueOrDefault("updated_at"),
            payment_methods = Array.Empty<object>()
        };
    }
}
