using Solid.Api.Common;

namespace Solid.Api.Features.Users;

public static class InstructorResource
{
    public static object From(Dictionary<string, object?> user)
    {
        return new
        {
            id = user["id"],
            display_name = user.GetValueOrDefault("display_name"),
            avatar_url = user.GetValueOrDefault("avatar_url"),
            bio = user.GetValueOrDefault("bio"),
            experience = user.JsonValue("experience") ?? Array.Empty<object>(),
            quote = user.GetValueOrDefault("quote")
        };
    }
}
