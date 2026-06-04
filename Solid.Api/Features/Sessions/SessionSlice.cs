using Microsoft.AspNetCore.Mvc;
using Solid.Api.Common;
using Solid.Api.Database.Entities;
using Solid.Api.Database.Repositories;
using Solid.Api.Infrastructure.Auth;

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

    private static async Task<IResult> Index(IAuthContext auth, ISessionRepository sessionRepository)
    {
        var sessions = await sessionRepository.ListForUserAsync(auth.UserId);

        return ApiResponse.Ok(new { sessions = sessions.Select(SessionResource.From) });
    }

    private static async Task<IResult> MeUpcomingSessions(IAuthContext auth, ISessionRepository sessionRepository)
    {
        var sessions = await sessionRepository.PaidForUserAsync(auth.UserId);

        return ApiResponse.Ok(new { sessions = sessions.Select(SessionResource.From) });
    }

    private static async Task<IResult> Show(long sessionId, ISessionRepository sessionRepository)
    {
        var session = await sessionRepository.FindAsync(sessionId);

        return session is null
            ? ApiResponse.Fail("Not found.", StatusCodes.Status404NotFound)
            : ApiResponse.Ok(new { session = SessionResource.From(session) });
    }

    private static async Task<IResult> Join(long sessionId, IAuthContext auth, ISessionRepository sessionRepository, IConfiguration configuration)
    {
        var session = await sessionRepository.FindAsync(sessionId);
        if (session is null)
        {
            return ApiResponse.Fail("Not found.", StatusCodes.Status404NotFound);
        }

        if (session.Status != "live")
        {
            return ApiResponse.Fail("Session is not live.", StatusCodes.Status422UnprocessableEntity);
        }

        await sessionRepository.RecordJoinAsync(sessionId, auth.UserId);

        return ApiResponse.Ok(new
        {
            jitsi_jwt = Hashing.RandomToken(48),
            jitsi_room_name = session.JitsiRoomName,
            jitsi_server_url = configuration["Jitsi:ServerUrl"] ?? "",
            session_duration_minutes = session.DurationMinutes
        });
    }

    private static async Task<IResult> Leave(long sessionId, IAuthContext auth, ISessionRepository sessionRepository)
    {
        await sessionRepository.LeaveAsync(sessionId, auth.UserId);

        return ApiResponse.Ok(message: "Left session successfully.");
    }

    private static async Task<IResult> Start(long sessionId, ISessionRepository sessionRepository, IConfiguration configuration)
    {
        await sessionRepository.StartAsync(sessionId);
        var session = await sessionRepository.FindAsync(sessionId);
        if (session is null)
        {
            return ApiResponse.Fail("Not found.", StatusCodes.Status404NotFound);
        }

        return ApiResponse.Ok(new
        {
            session = SessionResource.From(session),
            jitsi_jwt = Hashing.RandomToken(48),
            jitsi_room_name = session.JitsiRoomName,
            jitsi_server_url = configuration["Jitsi:ServerUrl"] ?? "",
            session_duration_minutes = session.DurationMinutes
        }, "Session started.");
    }

    private static async Task<IResult> End(long sessionId, ISessionRepository sessionRepository)
    {
        await sessionRepository.EndAsync(sessionId);

        return ApiResponse.Ok(message: "Session ended.");
    }

    private static async Task<IResult> Feedback(long sessionId, IAuthContext auth, [FromBody] FeedbackRequest request, ISessionRepository sessionRepository)
    {
        var attendance = await sessionRepository.SaveFeedbackAsync(sessionId, auth.UserId, request.rating, request.comment);

        return ApiResponse.Ok(new { attendance = attendance is null ? null : AttendanceResource(attendance) }, "Feedback submitted successfully.");
    }

    private static object AttendanceResource(SessionAttendance attendance)
    {
        return new
        {
            id = attendance.Id,
            session_id = attendance.SessionId,
            user_id = attendance.UserId,
            joined_at = attendance.JoinedAt,
            left_at = attendance.LeftAt,
            was_present = attendance.WasPresent,
            rating = attendance.Rating,
            comment = attendance.Comment,
            created_at = attendance.CreatedAt,
            updated_at = attendance.UpdatedAt
        };
    }
}

public sealed record FeedbackRequest(int rating, string? comment);
