using Microsoft.AspNetCore.Mvc;
using Solid.Api.Common;
using Solid.Api.Features.Shared;
using Solid.Api.Infrastructure.Auth;
using Solid.Api.Database;

namespace Solid.Api.Features.Users;

public static class UserSlice
{
    public static IEndpointRouteBuilder MapUserSlice(this IEndpointRouteBuilder api)
    {
        api.MapGet("/user", CurrentUser);
        api.MapGet("/users/{userId:long}", Show);
        api.MapPut("/profile", UpdateProfile);
        api.MapGet("/instructors", Instructors);
        api.MapGet("/instructors/{userId:long}", ShowInstructor);

        return api;
    }

    private static async Task<IResult> CurrentUser(IAuthContext auth, IDatabase database)
    {
        return ApiResponse.Ok(await database.UserAsync(auth.UserId));
    }

    private static async Task<IResult> Show(long userId, IDatabase database)
    {
        var user = await database.UserAsync(userId);

        return user is null
            ? ApiResponse.Fail("Not found.", StatusCodes.Status404NotFound)
            : ApiResponse.Ok(new { user = UserResource.From(user) });
    }

    private static async Task<IResult> UpdateProfile(IAuthContext auth, [FromBody] ProfileRequest request, IDatabase database)
    {
        await database.ExecuteAsync(
            """
            UPDATE users
            SET display_name = COALESCE(@display_name, display_name),
                email = COALESCE(@email, email),
                mobile_number = COALESCE(@mobile_number, mobile_number),
                bio = COALESCE(@bio, bio),
                avatar_url = COALESCE(@avatar_url, avatar_url),
                updated_at = SYSDATETIME()
            WHERE id = @userId
            """,
            new { auth.UserId, request.display_name, request.email, request.mobile_number, request.bio, request.avatar_url });

        return ApiResponse.Ok(new { user = UserResource.From((await database.UserAsync(auth.UserId))!) }, "Profile updated successfully.");
    }

    private static async Task<IResult> Instructors(IDatabase database)
    {
        var instructors = await database.QueryAsync("SELECT * FROM users WHERE [role] = 'instructor' AND deleted_at IS NULL ORDER BY id");

        return ApiResponse.Ok(new { instructors = instructors.Select(InstructorResource.From) });
    }

    private static async Task<IResult> ShowInstructor(long userId, IDatabase database)
    {
        var user = await database.UserAsync(userId);
        if (user is null || Convert.ToString(user["role"]) != "instructor")
        {
            return ApiResponse.Fail("User is not an instructor.", StatusCodes.Status404NotFound);
        }

        return ApiResponse.Ok(new { instructor = InstructorResource.From(user) });
    }
}

public sealed record ProfileRequest(string? display_name, string? email, string? mobile_number, string? bio, string? avatar_url);
