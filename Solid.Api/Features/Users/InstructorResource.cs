using Solid.Api.Common;
using Solid.Api.Database.Entities;
using Twilio.Types;

namespace Solid.Api.Features.Users;

public static class InstructorResource
{
    public static object From(User user)
    {
        return new
        {
            id = user.Id,
            display_name = user.DisplayName,
            PhoneNumber= user.MobileNumber,
            Email=user.Email,
            avatar_url = user.AvatarUrl,
            bio = user.Bio,
            experience = JsonPayload.Parse(user.Experience) ?? Array.Empty<object>(),
            quote = user.Quote
        };
    }
}
