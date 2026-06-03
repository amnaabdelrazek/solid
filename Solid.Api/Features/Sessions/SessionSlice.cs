using Microsoft.AspNetCore.Mvc;
using Solid.Api.Common;
using Solid.Api.Infrastructure.Auth;
using Solid.Api.Database;

namespace Solid.Api.Features.Sessions;

public static class SessionSlice
{
    public static IEndpointRouteBuilder MapSessionSlice(this IEndpointRouteBuilder api)
    {
        api.MapGet("/sessions", Index);
        api.MapGet("/sessions/me", MeUpcomingSessions);
        api.MapGet("/sessions/{sessionId:long}", Show);
        api.MapPost("/sessions/{sessionId:long}/join", Join);
        api.MapPost("/sessions/{sessionId:long}/leave", Leave);
        api.MapPost("/sessions/{sessionId:long}/start", Start);
        api.MapPost("/sessions/{sessionId:long}/end", End);
        api.MapPost("/sessions/{sessionId:long}/feedback", Feedback);

        return api;
    }

    private static async Task<IResult> Index(IAuthContext auth, IDatabase database)
    {
        var sessions = await database.QueryAsync(
            """
            SELECT ts.*
            FROM therapy_sessions ts
            LEFT JOIN group_members gm ON gm.group_id = ts.group_id
            WHERE (ts.instructor_id = @userId OR (gm.user_id = @userId AND gm.is_active = 1))
              AND ts.[status] <> 'finished'
              AND ts.deleted_at IS NULL
            ORDER BY ts.session_number, ts.scheduled_at
            """,
            new { auth.UserId });

        return ApiResponse.Ok(new { sessions = sessions.Select(SessionResource.From) });
    }

    private static async Task<IResult> MeUpcomingSessions(IAuthContext auth, IDatabase database)
    {
        var sessions = await database.QueryAsync(
            """
            SELECT ts.*
            FROM therapy_sessions ts
            INNER JOIN payments p ON p.session_id = ts.id
            WHERE p.user_id = @userId AND p.[status] = 'paid' AND ts.deleted_at IS NULL
            ORDER BY ts.scheduled_at
            OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY
            """,
            new { auth.UserId });

        return ApiResponse.Ok(new { sessions = sessions.Select(SessionResource.From) });
    }

    private static async Task<IResult> Show(long sessionId, IDatabase database)
    {
        var session = await database.QuerySingleAsync("SELECT TOP 1 * FROM therapy_sessions WHERE id = @sessionId AND deleted_at IS NULL", new { sessionId });

        return session is null
            ? ApiResponse.Fail("Not found.", StatusCodes.Status404NotFound)
            : ApiResponse.Ok(new { session = SessionResource.From(session) });
    }

    private static async Task<IResult> Join(long sessionId, IAuthContext auth, IDatabase database, IConfiguration configuration)
    {
        var session = await database.QuerySingleAsync("SELECT TOP 1 * FROM therapy_sessions WHERE id = @sessionId AND deleted_at IS NULL", new { sessionId });
        if (session is null)
        {
            return ApiResponse.Fail("Not found.", StatusCodes.Status404NotFound);
        }

        if (Convert.ToString(session["status"]) != "live")
        {
            return ApiResponse.Fail("Session is not live.", StatusCodes.Status422UnprocessableEntity);
        }

        await database.ExecuteAsync(
            """
            MERGE session_attendances AS target
            USING (SELECT @sessionId AS session_id, @userId AS user_id) AS source
            ON target.session_id = source.session_id AND target.user_id = source.user_id
            WHEN MATCHED THEN UPDATE SET joined_at = SYSDATETIME(), was_present = 1, updated_at = SYSDATETIME()
            WHEN NOT MATCHED THEN INSERT (session_id, user_id, joined_at, was_present, created_at, updated_at)
            VALUES (@sessionId, @userId, SYSDATETIME(), 1, SYSDATETIME(), SYSDATETIME());
            """,
            new { sessionId, auth.UserId });

        return ApiResponse.Ok(new
        {
            jitsi_jwt = Hashing.RandomToken(48),
            jitsi_room_name = session["jitsi_room_name"],
            jitsi_server_url = configuration["Jitsi:ServerUrl"] ?? "",
            session_duration_minutes = session["duration_minutes"]
        });
    }

    private static async Task<IResult> Leave(long sessionId, IAuthContext auth, IDatabase database)
    {
        await database.ExecuteAsync(
            "UPDATE session_attendances SET left_at = SYSDATETIME(), updated_at = SYSDATETIME() WHERE session_id = @sessionId AND user_id = @userId AND left_at IS NULL",
            new { sessionId, auth.UserId });

        return ApiResponse.Ok(message: "Left session successfully.");
    }

    private static async Task<IResult> Start(long sessionId, IDatabase database, IConfiguration configuration)
    {
        await database.ExecuteAsync("UPDATE therapy_sessions SET [status] = 'live', started_at = SYSDATETIME(), updated_at = SYSDATETIME() WHERE id = @sessionId", new { sessionId });
        var session = (await database.QuerySingleAsync("SELECT TOP 1 * FROM therapy_sessions WHERE id = @sessionId", new { sessionId }))!;

        return ApiResponse.Ok(new
        {
            session = SessionResource.From(session),
            jitsi_jwt = Hashing.RandomToken(48),
            jitsi_room_name = session["jitsi_room_name"],
            jitsi_server_url = configuration["Jitsi:ServerUrl"] ?? "",
            session_duration_minutes = session["duration_minutes"]
        }, "Session started.");
    }

    private static async Task<IResult> End(long sessionId, IDatabase database)
    {
        await database.ExecuteAsync("UPDATE therapy_sessions SET [status] = 'finished', ended_at = SYSDATETIME(), updated_at = SYSDATETIME() WHERE id = @sessionId", new { sessionId });

        return ApiResponse.Ok(message: "Session ended.");
    }

    private static async Task<IResult> Feedback(long sessionId, IAuthContext auth, [FromBody] FeedbackRequest request, IDatabase database)
    {
        await database.ExecuteAsync(
            "UPDATE session_attendances SET rating = @rating, comment = @comment, updated_at = SYSDATETIME() WHERE session_id = @sessionId AND user_id = @userId",
            new { request.rating, request.comment, sessionId, auth.UserId });
        var attendance = await database.QuerySingleAsync("SELECT TOP 1 * FROM session_attendances WHERE session_id = @sessionId AND user_id = @userId", new { sessionId, auth.UserId });

        return ApiResponse.Ok(new { attendance }, "Feedback submitted successfully.");
    }
}

public sealed record FeedbackRequest(int rating, string? comment);
