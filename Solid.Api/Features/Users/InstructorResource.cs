using Solid.Api.Common;
using Solid.Api.Database.Entities;

namespace Solid.Api.Features.Users;

public static class InstructorResource
{
    public static object From(User user)
    {
        return new
        {
            id = user.Id,
            display_name = user.DisplayName,
            phone_number = user.MobileNumber,   // fixed: was PhoneNumber (PascalCase)
            email = user.Email,                  // fixed: was Email (PascalCase)
            avatar_url = user.AvatarUrl,
            bio = user.Bio,
            experience = JsonPayload.Parse(user.Experience) ?? Array.Empty<object>(),
            quote = user.Quote
        };
    }
}
