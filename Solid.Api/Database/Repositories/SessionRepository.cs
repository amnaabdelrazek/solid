namespace Solid.Api.Database.Repositories;

public sealed class SessionRepository(IDatabase database) : ISessionRepository
{
    public async Task<IReadOnlyList<Dictionary<string, object?>>> ListForUserAsync(long userId)
    {
        return await database.QueryAsync(
            """
            SELECT ts.*
            FROM therapy_sessions ts
            LEFT JOIN group_members gm ON gm.group_id = ts.group_id
            WHERE (ts.instructor_id = @userId OR (gm.user_id = @userId AND gm.is_active = 1))
              AND ts.[status] <> 'finished'
              AND ts.deleted_at IS NULL
            ORDER BY ts.session_number, ts.scheduled_at
            """,
            new { userId });
    }

    public async Task<IReadOnlyList<Dictionary<string, object?>>> PaidForUserAsync(long userId)
    {
        return await database.QueryAsync(
            """
            SELECT ts.*
            FROM therapy_sessions ts
            INNER JOIN payments p ON p.session_id = ts.id
            WHERE p.user_id = @userId AND p.[status] = 'paid' AND ts.deleted_at IS NULL
            ORDER BY ts.scheduled_at
            OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY
            """,
            new { userId });
    }

    public async Task<Dictionary<string, object?>?> FindAsync(long sessionId)
    {
        return await database.QuerySingleAsync("SELECT TOP 1 * FROM therapy_sessions WHERE id = @sessionId AND deleted_at IS NULL", new { sessionId });
    }

    public async Task RecordJoinAsync(long sessionId, long userId)
    {
        await database.ExecuteAsync(
            """
            MERGE session_attendances AS target
            USING (SELECT @sessionId AS session_id, @userId AS user_id) AS source
            ON target.session_id = source.session_id AND target.user_id = source.user_id
            WHEN MATCHED THEN UPDATE SET joined_at = SYSDATETIME(), was_present = 1, updated_at = SYSDATETIME()
            WHEN NOT MATCHED THEN INSERT (session_id, user_id, joined_at, was_present, created_at, updated_at)
            VALUES (@sessionId, @userId, SYSDATETIME(), 1, SYSDATETIME(), SYSDATETIME());
            """,
            new { sessionId, userId });
    }

    public async Task LeaveAsync(long sessionId, long userId)
    {
        await database.ExecuteAsync(
            "UPDATE session_attendances SET left_at = SYSDATETIME(), updated_at = SYSDATETIME() WHERE session_id = @sessionId AND user_id = @userId AND left_at IS NULL",
            new { sessionId, userId });
    }

    public async Task StartAsync(long sessionId)
    {
        await database.ExecuteAsync("UPDATE therapy_sessions SET [status] = 'live', started_at = SYSDATETIME(), updated_at = SYSDATETIME() WHERE id = @sessionId", new { sessionId });
    }

    public async Task EndAsync(long sessionId)
    {
        await database.ExecuteAsync("UPDATE therapy_sessions SET [status] = 'finished', ended_at = SYSDATETIME(), updated_at = SYSDATETIME() WHERE id = @sessionId", new { sessionId });
    }

    public async Task<Dictionary<string, object?>?> SaveFeedbackAsync(long sessionId, long userId, int rating, string? comment)
    {
        await database.ExecuteAsync(
            "UPDATE session_attendances SET rating = @rating, comment = @comment, updated_at = SYSDATETIME() WHERE session_id = @sessionId AND user_id = @userId",
            new { rating, comment, sessionId, userId });

        return await database.QuerySingleAsync("SELECT TOP 1 * FROM session_attendances WHERE session_id = @sessionId AND user_id = @userId", new { sessionId, userId });
    }
}
