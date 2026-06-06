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
        api.MapPost("/instructors", CreateInstructor);

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

    private static async Task<IResult> CreateInstructor(
        IAuthContext auth,
        [FromBody] CreateInstructorRequest request,
        IUserRepository userRepository)
    {
        //if (!auth.IsAdmin())
        //{
        //    return ApiResponse.Fail("This action is unauthorized.", StatusCodes.Status403Forbidden);
        //}

        if (string.IsNullOrWhiteSpace(request.display_name) ||
            string.IsNullOrWhiteSpace(request.mobile_number) ||
            string.IsNullOrWhiteSpace(request.password))
        {
            return ApiResponse.Fail("The given data was invalid.", StatusCodes.Status422UnprocessableEntity);
        }

        if (!PhoneNumberValidator.TryNormalize(request.mobile_number, out var normalizedMobileNumber))
        {
            return ApiResponse.Fail(PhoneNumberValidator.Message, StatusCodes.Status422UnprocessableEntity);
        }

        if (await userRepository.FindByMobileAsync(normalizedMobileNumber) is not null)
        {
            return ApiResponse.Fail("Mobile number already exists.", StatusCodes.Status422UnprocessableEntity);
        }

        var instructor = await userRepository.CreateInstructorAsync(new InstructorCreate(
            request.display_name,
            normalizedMobileNumber,
            BCrypt.Net.BCrypt.HashPassword(request.password, 12),
            request.email,
            request.bio,
            request.avatar_url,
            request.experience,
            request.quote,
            request.preferred_language ?? "ar"));

        return ApiResponse.Ok(new { instructor = InstructorResource.From(instructor) }, "Instructor created successfully.");
    }
}

public sealed record ProfileRequest(string? display_name, string? email, string? mobile_number, string? bio, string? avatar_url);

public sealed record CreateInstructorRequest(
    string display_name,
    string mobile_number,
    string password,
    string? email,
    string? bio,
    string? avatar_url,
    string? experience,
    string? quote,
    string? preferred_language);
