using Solid.Api.Common;
using Solid.Api.Database.Entities;

namespace Solid.Api.Features.Users;

public static class InstructorResource
{
    public static object From(User user)
    {
        return From(user, null);
    }

    public static object From(User user, decimal? sessionPrice)
    {
        return new
        {
            id = user.Id,
            display_name = user.DisplayName,
            phone_number = user.MobileNumber,
            email = user.Email,
            avatar_url = user.AvatarUrl,
            bio = user.Bio,
            experience = JsonPayload.Parse(user.Experience) ?? Array.Empty<object>(),
            quote = user.Quote,
            session_price = sessionPrice,
            formatted_session_price = sessionPrice is null ? null : $"{sessionPrice:0.##} EGP"
        };
    }
}
