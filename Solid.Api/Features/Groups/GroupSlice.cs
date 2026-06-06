using Solid.Api.Common;
using Solid.Api.Database.Repositories;
using Solid.Api.Infrastructure.Auth;

namespace Solid.Api.Features.Groups;

public static class GroupSlice
{
    public static IEndpointRouteBuilder MapGroupSlice(this IEndpointRouteBuilder api)
    {
        api.MapGet("/groups/my", MyGroup);
        api.MapGet("/groups", Index);
        api.MapPost("/groups/subscribe", Subscribe);

        return api;
    }

    private static async Task<IResult> Index(IAuthContext auth, IGroupRepository groupRepository)
    {
        if (!auth.IsAdminOrInstructor())
        {
            return ApiResponse.Fail("This action is unauthorized.", StatusCodes.Status403Forbidden);
        }

        var groups = await groupRepository.ListAsync();

        return ApiResponse.Ok(new { groups = groups.Select(GroupResource.From) });
    }

    private static async Task<IResult> MyGroup(IAuthContext auth, IGroupRepository groupRepository)
    {
        var group = await groupRepository.MyGroupAsync(auth.UserId);

        return group is null
            ? ApiResponse.Fail("Not found.", StatusCodes.Status404NotFound)
            : ApiResponse.Ok(new { group = GroupResource.From(group) });
    }

    private static async Task<IResult> Subscribe(IAuthContext auth, IGroupRepository groupRepository)
    {
        if (await groupRepository.HasActiveMembershipAsync(auth.UserId))
        {
            return ApiResponse.Fail("You are already subscribed to a group.", StatusCodes.Status422UnprocessableEntity);
        }

        var group = await groupRepository.FindOrCreateForUserSubstanceAsync(auth.UserId);
        if (group is null)
        {
            return ApiResponse.Fail("You must choose a substance before joining a group.", StatusCodes.Status422UnprocessableEntity);
        }

        await groupRepository.AddMemberAsync(group.Id, auth.UserId);

        return ApiResponse.Ok(new { group = GroupResource.From(group) }, "Joined group successfully. Waiting for other members to start sessions.");
    }
}
