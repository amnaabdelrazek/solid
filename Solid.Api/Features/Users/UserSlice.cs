using Microsoft.AspNetCore.Mvc;
using Solid.Api.Common;
using Solid.Api.Database.Repositories;
using Solid.Api.Features.Shared;
using Solid.Api.Infrastructure.Auth;

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

    private static async Task<IResult> CurrentUser(IAuthContext auth, IUserRepository userRepository)
    {
        var user = await userRepository.FindAsync(auth.UserId);

        return user is null
            ? ApiResponse.Fail("Not found.", StatusCodes.Status404NotFound)
            : ApiResponse.Ok(UserResource.From(user));
    }

    private static async Task<IResult> Show(long userId, IUserRepository userRepository)
    {
        var user = await userRepository.FindAsync(userId);

        return user is null
            ? ApiResponse.Fail("Not found.", StatusCodes.Status404NotFound)
            : ApiResponse.Ok(new { user = UserResource.From(user) });
    }

    private static async Task<IResult> UpdateProfile(IAuthContext auth, [FromBody] ProfileRequest request, IUserRepository userRepository)
    {
        await userRepository.UpdateProfileAsync(
            auth.UserId,
            new ProfileUpdate(request.display_name, request.email, request.mobile_number, request.bio, request.avatar_url));

        var user = await userRepository.FindAsync(auth.UserId);

        return ApiResponse.Ok(new { user = UserResource.From(user!) }, "Profile updated successfully.");
    }

    private static async Task<IResult> Instructors(IUserRepository userRepository)
    {
        var instructors = await userRepository.InstructorsAsync();

        return ApiResponse.Ok(new { instructors = instructors.Select(InstructorResource.From) });
    }

    private static async Task<IResult> ShowInstructor(long userId, IUserRepository userRepository)
    {
        var user = await userRepository.FindAsync(userId);
        if (user is null || user.Role != "instructor")
        {
            return ApiResponse.Fail("User is not an instructor.", StatusCodes.Status404NotFound);
        }

        return ApiResponse.Ok(new { instructor = InstructorResource.From(user) });
    }
}

public sealed record ProfileRequest(string? display_name, string? email, string? mobile_number, string? bio, string? avatar_url);
