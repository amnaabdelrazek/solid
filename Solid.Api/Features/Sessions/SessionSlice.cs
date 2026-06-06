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
        api.MapPost("/sessions", Create);
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
        var sessions = await sessionRepository.ListForUserAsync(auth.UserId, auth.Role);

        return ApiResponse.Ok(new { sessions = sessions.Select(SessionResource.From) });
    }

    private static async Task<IResult> Create(
        IAuthContext auth,
        [FromBody] CreateSessionRequest request,
        ISessionRepository sessionRepository)
    {
        if (!auth.IsAdminOrInstructor())
        {
            return ApiResponse.Fail("This action is unauthorized.", StatusCodes.Status403Forbidden);
        }

        if (request.group_id <= 0 || request.scheduled_at == default)
        {
            return ApiResponse.Fail("The given data was invalid.", StatusCodes.Status422UnprocessableEntity);
        }

        var instructorId = auth.IsInstructor()
            ? auth.UserId
            : request.instructor_id;

        if (instructorId is null or <= 0)
        {
            return ApiResponse.Fail("Instructor is required.", StatusCodes.Status422UnprocessableEntity);
        }

        var result = await sessionRepository.CreateAsync(new SessionCreate(
            request.group_id,
            instructorId.Value,
            request.session_number,
            request.title,
            request.session_type ?? "group",
            request.scheduled_at,
            request.duration_minutes ?? 45,
            request.max_participants,
            request.metadata));

        if (result.Session is null)
        {
            return ApiResponse.Fail(result.Error ?? "Unable to create session.", result.StatusCode);
        }

        return ApiResponse.Ok(new { session = SessionResource.From(result.Session) }, "Session created successfully.");
    }

    private static async Task<IResult> MeUpcomingSessions(IAuthContext auth, ISessionRepository sessionRepository)
    {
        var sessions = await sessionRepository.PaidForUserAsync(auth.UserId);

        return ApiResponse.Ok(new { sessions = sessions.Select(SessionResource.From) });
    }

    private static async Task<IResult> Show(long sessionId, IAuthContext auth, ISessionRepository sessionRepository)
    {
        var session = await sessionRepository.FindAsync(sessionId, auth.UserId, auth.Role);

        return session is null
            ? ApiResponse.Fail("Not found.", StatusCodes.Status404NotFound)
            : ApiResponse.Ok(new { session = SessionResource.From(session) });
    }

    private static async Task<IResult> Join(long sessionId, IAuthContext auth, ISessionRepository sessionRepository, IConfiguration configuration)
    {
        var result = await sessionRepository.RecordJoinAsync(sessionId, auth.UserId);
        if (!result.Success || result.Session is null)
        {
            return ApiResponse.Fail(result.Error ?? "Unable to join session.", result.StatusCode);
        }

        return ApiResponse.Ok(new
        {
            jitsi_jwt = Hashing.RandomToken(48),
            jitsi_room_name = result.Session.JitsiRoomName,
            jitsi_server_url = configuration["Jitsi:ServerUrl"] ?? "",
            session_duration_minutes = result.Session.DurationMinutes
        });
    }

    private static async Task<IResult> Leave(long sessionId, IAuthContext auth, ISessionRepository sessionRepository)
    {
        await sessionRepository.LeaveAsync(sessionId, auth.UserId);

        return ApiResponse.Ok(message: "Left session successfully.");
    }

    private static async Task<IResult> Start(long sessionId, IAuthContext auth, ISessionRepository sessionRepository, IConfiguration configuration)
    {
        if (!auth.IsAdminOrInstructor())
        {
            return ApiResponse.Fail("This action is unauthorized.", StatusCodes.Status403Forbidden);
        }

        await sessionRepository.StartAsync(sessionId);
        var session = await sessionRepository.FindAnyAsync(sessionId);
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

    private static async Task<IResult> End(long sessionId, IAuthContext auth, ISessionRepository sessionRepository)
    {
        if (!auth.IsAdminOrInstructor())
        {
            return ApiResponse.Fail("This action is unauthorized.", StatusCodes.Status403Forbidden);
        }

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

public sealed record CreateSessionRequest(
    long group_id,
    long? instructor_id,
    int? session_number,
    string? title,
    string? session_type,
    DateTime scheduled_at,
    byte? duration_minutes,
    int? max_participants,
    object? metadata);

public sealed record FeedbackRequest(int rating, string? comment);
