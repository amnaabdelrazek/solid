using Solid.Api.Common;
using Solid.Api.Infrastructure.Auth;
using Solid.Api.Database;

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

    private static async Task<IResult> Index(IDatabase database)
    {
        var groups = await database.QueryAsync("SELECT * FROM groups WHERE deleted_at IS NULL ORDER BY id DESC");

        return ApiResponse.Ok(new { groups = groups.Select(GroupResource.From) });
    }

    private static async Task<IResult> MyGroup(IAuthContext auth, IDatabase database)
    {
        var group = await database.QuerySingleAsync(
            """
            SELECT TOP 1 g.*
            FROM groups g
            INNER JOIN group_members gm ON gm.group_id = g.id
            WHERE gm.user_id = @userId AND gm.is_active = 1 AND g.deleted_at IS NULL
            ORDER BY gm.joined_at DESC
            """,
            new { auth.UserId });

        return group is null
            ? ApiResponse.Fail("Not found.", StatusCodes.Status404NotFound)
            : ApiResponse.Ok(new { group = GroupResource.From(group) });
    }

    private static async Task<IResult> Subscribe(IAuthContext auth, IDatabase database)
    {
        var existing = await database.QuerySingleAsync("SELECT TOP 1 id FROM group_members WHERE user_id = @userId AND is_active = 1", new { auth.UserId });
        if (existing is not null)
        {
            return ApiResponse.Fail("You are already subscribed to a group.", StatusCodes.Status422UnprocessableEntity);
        }

        var group = await database.QuerySingleAsync("SELECT TOP 1 * FROM groups WHERE [status] = 'forming' AND deleted_at IS NULL ORDER BY id");
        if (group is null)
        {
            var groupId = await database.ExecuteScalarAsync<long>(
                """
                INSERT INTO groups (group_type, [status], name_ar, name_en, min_members, max_members, created_at, updated_at)
                OUTPUT INSERTED.id
                VALUES ('mixed', 'forming', N'مجموعة جديدة', 'New Group', 7, 15, SYSDATETIME(), SYSDATETIME())
                """);
            group = (await database.QuerySingleAsync("SELECT TOP 1 * FROM groups WHERE id = @groupId", new { groupId }))!;
        }

        await database.ExecuteAsync(
            "INSERT INTO group_members (group_id, user_id, joined_at, is_active) VALUES (@groupId, @userId, SYSDATETIME(), 1)",
            new { groupId = group["id"], auth.UserId });

        return ApiResponse.Ok(new { group = GroupResource.From(group) }, "Joined group successfully. Waiting for other members to start sessions.");
    }
}
