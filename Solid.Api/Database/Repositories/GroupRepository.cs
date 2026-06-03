namespace Solid.Api.Database.Repositories;

public sealed class GroupRepository(IDatabase database) : IGroupRepository
{
    public async Task<IReadOnlyList<Dictionary<string, object?>>> ListAsync()
    {
        return await database.QueryAsync("SELECT * FROM groups WHERE deleted_at IS NULL ORDER BY id DESC");
    }

    public async Task<Dictionary<string, object?>?> MyGroupAsync(long userId)
    {
        return await database.QuerySingleAsync(
            """
            SELECT TOP 1 g.*
            FROM groups g
            INNER JOIN group_members gm ON gm.group_id = g.id
            WHERE gm.user_id = @userId AND gm.is_active = 1 AND g.deleted_at IS NULL
            ORDER BY gm.joined_at DESC
            """,
            new { userId });
    }

    public async Task<bool> HasActiveMembershipAsync(long userId)
    {
        return await database.QuerySingleAsync("SELECT TOP 1 id FROM group_members WHERE user_id = @userId AND is_active = 1", new { userId }) is not null;
    }

    public async Task<Dictionary<string, object?>> FindOrCreateOpenAsync()
    {
        var group = await database.QuerySingleAsync("SELECT TOP 1 * FROM groups WHERE [status] = 'forming' AND deleted_at IS NULL ORDER BY id");
        if (group is not null)
        {
            return group;
        }

        var groupId = await database.ExecuteScalarAsync<long>(
            """
            INSERT INTO groups (group_type, [status], name_ar, name_en, min_members, max_members, created_at, updated_at)
            OUTPUT INSERTED.id
            VALUES ('mixed', 'forming', N'مجموعة جديدة', 'New Group', 7, 15, SYSDATETIME(), SYSDATETIME())
            """);

        return (await database.QuerySingleAsync("SELECT TOP 1 * FROM groups WHERE id = @groupId", new { groupId }))!;
    }

    public async Task AddMemberAsync(object groupId, long userId)
    {
        await database.ExecuteAsync(
            "INSERT INTO group_members (group_id, user_id, joined_at, is_active) VALUES (@groupId, @userId, SYSDATETIME(), 1)",
            new { groupId, userId });
    }
}
