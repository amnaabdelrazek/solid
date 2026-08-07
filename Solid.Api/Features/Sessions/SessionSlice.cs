using Microsoft.AspNetCore.Mvc;
using Solid.Api.Common;
using Solid.Api.Database.Entities;
using Solid.Api.Database.Repositories;
using Solid.Api.Features.Notifications;
using Solid.Api.Infrastructure.Auth;
using Solid.Api.Infrastructure.Jitsi;
using Stripe;

namespace Solid.Api.Features.Sessions;

public static class SessionSlice
{
    public static IEndpointRouteBuilder MapSessionSlice(this IEndpointRouteBuilder api)
    {
        api.MapGet("/sessions", Index);
        api.MapPost("/sessions", Create)
            .Accepts<CreateSessionRequest>("application/json", "application/x-www-form-urlencoded", "multipart/form-data");
        api.MapGet("/sessions/upcoming", Upcoming);
        api.MapGet("/sessions/upcoming/individual", UpcomingIndividual);
        api.MapGet("/sessions/upcoming/group", UpcomingGroup);
        api.MapGet("/sessions/upcoming/unpaid", UpcomingUnpaid);
        api.MapGet("/sessions/upcoming/unpaid/individual", UpcomingUnpaidIndividual);
        api.MapGet("/sessions/upcoming/unpaid/group", UpcomingUnpaidGroup);
        api.MapGet("/sessions/me", Upcoming);
        api.MapGet("/sessions/{sessionId:long}", Show);
        api.MapGet("/sessions/attended", Attended);
        api.MapGet("/sessions/{sessionId:long}/ratings", Ratings);
        api.MapPost("/sessions/{sessionId:long}/join", Join);
        api.MapPost("/sessions/{sessionId:long}/leave", Leave);
        api.MapPost("/sessions/{sessionId:long}/start", Start);
        api.MapPost("/sessions/{sessionId:long}/end", End);
        api.MapPost("/sessions/{sessionId:long}/feedback", Feedback)
            .Accepts<FeedbackRequest>("application/json", "application/x-www-form-urlencoded", "multipart/form-data");

        return api;
    }

    private static async Task<IResult> Index(
        IAuthContext auth,
        ISessionRepository sessionRepository,
        ISettingsRepository settingsRepository,
        int page = 1,
        int pageSize = 10)
    {
        if (page < 1)
        {
            page = 1;
        }
        if (pageSize < 1)
        {
            pageSize = 10;
        }
        if (pageSize > 100)
        {
            pageSize = 100;
        }

        var (sessions, totalCount) = await sessionRepository.ListForUserPaginatedAsync(auth.UserId, auth.Role, page, pageSize);
        var settingsAmount = await settingsRepository.GetAsync("general", "session_price");
        decimal.TryParse(settingsAmount, out var price);
        long? userId = auth.IsAdminOrInstructor() ? null : auth.UserId;

        return ApiResponse.Ok(new
        {
            sessions = sessions.Select(s => SessionResource.From(s, price, userId)),
            pagination = new Dictionary<string, int>
            {
                { "current_page", page },
                { "page_size", pageSize },
                { "total_count", totalCount },
                { "total_pages", (int)Math.Ceiling((double)totalCount / pageSize) }
            }
        });
    }

    private static async Task<IResult> Attended(
        IAuthContext auth,
        ISessionRepository sessionRepository,
        ISettingsRepository settingsRepository,
        int page = 1,
        int pageSize = 10)
    {
        if (page < 1)
        {
            page = 1;
        }
        if (pageSize < 1)
        {
            pageSize = 10;
        }
        if (pageSize > 100)
        {
            pageSize = 100;
        }

        var (sessions, totalCount) = await sessionRepository.AttendedSessionsForUserPaginatedAsync(auth.UserId, page, pageSize);

        var settingsAmount = await settingsRepository.GetAsync("general", "session_price");
        decimal.TryParse(settingsAmount, out var price);

        return ApiResponse.Ok(new
        {
            sessions = sessions.Select(s => AttendedSessionResource.From(s, auth.UserId, price)),
            pagination = new Dictionary<string, int>
            {
                { "current_page", page },
                { "page_size", pageSize },
                { "total_count", totalCount },
                { "total_pages", (int)Math.Ceiling((double)totalCount / pageSize) }
            }
        });
    }

    private static async Task<IResult> Create(
        IAuthContext auth,
        HttpRequest httpRequest,
        ISessionRepository sessionRepository,
        INotificationService notificationService)
    {
        var payload = await RequestPayload.ReadAsync<CreateSessionRequest>(httpRequest);
        if (payload.Error is not null)
            return payload.Error;

        var request = payload.Value!;

        var substanceCategoryId = request.substance_category_id ?? request.substance_categories_id ?? request.group_id;

        if (substanceCategoryId is null or <= 0 || request.scheduled_at == default)
            return ApiResponse.Fail("The given data was invalid.", StatusCodes.Status422UnprocessableEntity);

        var instructorId = auth.IsInstructor() ? auth.UserId : request.instructor_id;

        if (instructorId is null or <= 0)
            return ApiResponse.Fail("Instructor is required.", StatusCodes.Status422UnprocessableEntity);

        var scheduledAtUtc = EgyptDateTime.ToUtcFromEgyptClock(request.scheduled_at);
        var sessionType = NormalizeSessionType(request.session_type);

        if (sessionType is null)
            return ApiResponse.Fail("session_type must be either individual or group.", StatusCodes.Status422UnprocessableEntity);

        var durationMinutes = sessionType == "individual"
            ? (byte)20
            : request.duration_minutes ?? 45;

        var maxParticipants = sessionType == "individual"
            ? 1
            : Math.Min(request.max_participants ?? 15, 15);

        var result = await sessionRepository.CreateAsync(new SessionCreate(
            substanceCategoryId.Value,
            instructorId.Value,
            request.session_number,
            request.title,
            sessionType,
            scheduledAtUtc,
            durationMinutes,
            maxParticipants,
            request.metadata));

        if (result.Session is null)
            return ApiResponse.Fail(result.Error ?? "Unable to create session.", result.StatusCode);

        await notificationService.NotifyUsersAsync(
            await SessionRecipientsAsync(sessionRepository, result.Session, includeInstructor: true),
            "SessionCreated",
            "New session",
            $"Session #{result.Session.SessionNumber ?? result.Session.Id} has been scheduled.",
            "calendar",
            new
            {
                session_id = result.Session.Id,
                substance_category_id = result.Session.SubstanceCategoryId,
                scheduled_at = EgyptDateTime.Format(result.Session.ScheduledAt)
            });

        return ApiResponse.Ok(
            new { session = SessionResource.From(result.Session) },
            "Session created successfully.");
    }

    private static async Task<IResult> Ratings(
    long sessionId,
    IAuthContext auth,
    ISessionRepository sessionRepository)
    {
        var session = await sessionRepository.FindWithAttendeesAsync(sessionId);
        if (session is null)
            return ApiResponse.Fail("Not found.", StatusCodes.Status404NotFound);

        // بس الأدمن أو الإنستركتور بتاع السيشن دي يقدر يشوف كل التقييمات
        //if (!auth.IsAdminOrInstructor())
        //    return ApiResponse.Fail("This action is unauthorized.", StatusCodes.Status403Forbidden);

        //if (auth.IsInstructor() && session.InstructorId != auth.UserId)
        //    return ApiResponse.Fail("This action is unauthorized.", StatusCodes.Status403Forbidden);

        var ratedAttendances = session.Attendances
            .Where(attendance => attendance.Rating.HasValue)
            .OrderByDescending(attendance => attendance.UpdatedAt)
            .ToList();

        var ratings = ratedAttendances.Select(attendance => new
        {
            user_id = attendance.UserId,
            user_name = attendance.User?.DisplayName,
            rating = attendance.Rating,
            comment = attendance.Comment,
            rated_at = EgyptDateTime.Format(attendance.UpdatedAt)
        });

        double? averageRating = ratedAttendances.Count > 0
            ? Math.Round(ratedAttendances.Average(attendance => (double)attendance.Rating!.Value), 2)
            : null;

        return ApiResponse.Ok(new
        {
            session_id = session.Id,
            session_number = session.SessionNumber,
            total_attendees = session.Attendances.Count(attendance => attendance.WasPresent),
            total_ratings = ratedAttendances.Count,
            average_rating = averageRating,
            ratings
        });
    }

    private static async Task<IResult> Upcoming(
        IAuthContext auth,
        ISessionRepository sessionRepository,
        ISettingsRepository settingsRepository,
        int page = 1,
        int pageSize = 10)
    {
        if (page < 1)
        {
            page = 1;
        }
        if (pageSize < 1)
        {
            pageSize = 10;
        }
        if (pageSize > 100)
        {
            pageSize = 100;
        }

        var (sessions, totalCount) = await sessionRepository.UpcomingPaidForUserPaginatedAsync(auth.UserId, page: page, pageSize: pageSize);
        var settingsAmount = await settingsRepository.GetAsync("general", "session_price");
        decimal.TryParse(settingsAmount, out var price);
        long? userId = auth.IsAdminOrInstructor() ? null : auth.UserId;

        return ApiResponse.Ok(new
        {
            sessions = sessions.Select(s => SessionResource.From(s, price, userId)),
            pagination = new Dictionary<string, int>
            {
                { "current_page", page },
                { "page_size", pageSize },
                { "total_count", totalCount },
                { "total_pages", (int)Math.Ceiling((double)totalCount / pageSize) }
            }
        });
    }

    private static async Task<IResult> UpcomingIndividual(
        IAuthContext auth,
        ISessionRepository sessionRepository,
        ISettingsRepository settingsRepository,
        int page = 1,
        int pageSize = 10)
    {
        if (page < 1)
        {
            page = 1;
        }
        if (pageSize < 1)
        {
            pageSize = 10;
        }
        if (pageSize > 100)
        {
            pageSize = 100;
        }

        var (sessions, totalCount) = await sessionRepository.UpcomingPaidForUserPaginatedAsync(auth.UserId, "individual", page, pageSize);
        var settingsAmount = await settingsRepository.GetAsync("general", "session_price");
        decimal.TryParse(settingsAmount, out var price);
        long? userId = auth.IsAdminOrInstructor() ? null : auth.UserId;

        return ApiResponse.Ok(new
        {
            sessions = sessions.Select(s => SessionResource.From(s, price, userId)),
            pagination = new Dictionary<string, int>
            {
                { "current_page", page },
                { "page_size", pageSize },
                { "total_count", totalCount },
                { "total_pages", (int)Math.Ceiling((double)totalCount / pageSize) }
            }
        });
    }

    private static async Task<IResult> UpcomingGroup(
        IAuthContext auth,
        ISessionRepository sessionRepository,
        ISettingsRepository settingsRepository,
        int page = 1,
        int pageSize = 10)
    {
        if (page < 1)
        {
            page = 1;
        }
        if (pageSize < 1)
        {
            pageSize = 10;
        }
        if (pageSize > 100)
        {
            pageSize = 100;
        }

        var (sessions, totalCount) = await sessionRepository.UpcomingPaidForUserPaginatedAsync(auth.UserId, "group", page, pageSize);
        var settingsAmount = await settingsRepository.GetAsync("general", "session_price");
        decimal.TryParse(settingsAmount, out var price);
        long? userId = auth.IsAdminOrInstructor() ? null : auth.UserId;

        return ApiResponse.Ok(new
        {
            sessions = sessions.Select(s => SessionResource.From(s, price, userId)),
            pagination = new Dictionary<string, int>
            {
                { "current_page", page },
                { "page_size", pageSize },
                { "total_count", totalCount },
                { "total_pages", (int)Math.Ceiling((double)totalCount / pageSize) }
            }
        });
    }

    private static async Task<IResult> UpcomingUnpaid(
        IAuthContext auth,
        ISessionRepository sessionRepository,
        ISettingsRepository settingsRepository,
        int page = 1,
        int pageSize = 10)
    {
        if (page < 1)
        {
            page = 1;
        }
        if (pageSize < 1)
        {
            pageSize = 10;
        }
        if (pageSize > 100)
        {
            pageSize = 100;
        }

        var (sessions, totalCount) = await sessionRepository.UpcomingUnpaidForUserPaginatedAsync(auth.UserId, page: page, pageSize: pageSize);
        var settingsAmount = await settingsRepository.GetAsync("general", "session_price");
        decimal.TryParse(settingsAmount, out var price);
        long? userId = auth.IsAdminOrInstructor() ? null : auth.UserId;

        return ApiResponse.Ok(new
        {
            sessions = sessions.Select(s => SessionResource.From(s, price, userId)),
            pagination = new Dictionary<string, int>
            {
                { "current_page", page },
                { "page_size", pageSize },
                { "total_count", totalCount },
                { "total_pages", (int)Math.Ceiling((double)totalCount / pageSize) }
            }
        });
    }

    private static async Task<IResult> UpcomingUnpaidIndividual(
        IAuthContext auth,
        ISessionRepository sessionRepository,
        ISettingsRepository settingsRepository,
        int page = 1,
        int pageSize = 10)
    {
        if (page < 1)
        {
            page = 1;
        }
        if (pageSize < 1)
        {
            pageSize = 10;
        }
        if (pageSize > 100)
        {
            pageSize = 100;
        }

        var (sessions, totalCount) = await sessionRepository.UpcomingUnpaidForUserPaginatedAsync(auth.UserId, "individual", page, pageSize);
        var settingsAmount = await settingsRepository.GetAsync("general", "session_price");
        decimal.TryParse(settingsAmount, out var price);
        long? userId = auth.IsAdminOrInstructor() ? null : auth.UserId;

        return ApiResponse.Ok(new
        {
            sessions = sessions.Select(s => SessionResource.From(s, price, userId)),
            pagination = new Dictionary<string, int>
            {
                { "current_page", page },
                { "page_size", pageSize },
                { "total_count", totalCount },
                { "total_pages", (int)Math.Ceiling((double)totalCount / pageSize) }
            }
        });
    }

    private static async Task<IResult> UpcomingUnpaidGroup(
        IAuthContext auth,
        ISessionRepository sessionRepository,
        ISettingsRepository settingsRepository,
        int page = 1,
        int pageSize = 10)
    {
        if (page < 1)
        {
            page = 1;
        }
        if (pageSize < 1)
        {
            pageSize = 10;
        }
        if (pageSize > 100)
        {
            pageSize = 100;
        }

        var (sessions, totalCount) = await sessionRepository.UpcomingUnpaidForUserPaginatedAsync(auth.UserId, "group", page, pageSize);
        var settingsAmount = await settingsRepository.GetAsync("general", "session_price");
        decimal.TryParse(settingsAmount, out var price);
        long? userId = auth.IsAdminOrInstructor() ? null : auth.UserId;

        return ApiResponse.Ok(new
        {
            sessions = sessions.Select(s => SessionResource.From(s, price, userId)),
            pagination = new Dictionary<string, int>
            {
                { "current_page", page },
                { "page_size", pageSize },
                { "total_count", totalCount },
                { "total_pages", (int)Math.Ceiling((double)totalCount / pageSize) }
            }
        });
    }

    private static async Task<IResult> Show(
        long sessionId,
        IAuthContext auth,
        ISessionRepository sessionRepository,
        ISettingsRepository settingsRepository)
    {
        var session = await sessionRepository.FindAsync(sessionId, auth.UserId, auth.Role);
        var settingsAmount = await settingsRepository.GetAsync("general", "session_price");
        decimal.TryParse(settingsAmount, out var price);

        long? userId = auth.IsAdminOrInstructor() ? null : auth.UserId;
        return session is null
            ? ApiResponse.Fail("Not found.", StatusCodes.Status404NotFound)
            : ApiResponse.Ok(new { session = SessionResource.From(session, price, userId) });
    }

    // ── JOIN ─────────────────────────────────────────────────────────────────

    private static async Task<IResult> Join(
        long sessionId,
        IAuthContext auth,
        ISessionRepository sessionRepository,
        IUserRepository userRepository,
        IJaasTokenService jaasTokenService,
        IConfiguration configuration)
    {
        var result = await sessionRepository.RecordJoinAsync(sessionId, auth.UserId);
        if (!result.Success || result.Session is null)
            return ApiResponse.Fail(result.Error ?? "Unable to join session.", result.StatusCode);

        var user = await userRepository.FindAsync(auth.UserId);

        var (jwt, meetingUrl) = BuildJaasResponse(
            jaasTokenService,
            configuration,
            auth.UserId,
            user?.DisplayName ?? "User",
            user?.AvatarUrl,
            result.Session.JitsiRoomName,
            isModerator: false);

        return ApiResponse.Ok(new
        {
            jitsi_jwt = jwt,
            jitsi_room_name = result.Session.JitsiRoomName,
            jitsi_server_url = GetServerUrl(configuration),
            meeting_url = meetingUrl,
            session_duration_minutes = result.Session.DurationMinutes
        });
    }

    // ── LEAVE ────────────────────────────────────────────────────────────────

    private static async Task<IResult> Leave(
        long sessionId,
        IAuthContext auth,
        ISessionRepository sessionRepository)
    {
        await sessionRepository.LeaveAsync(sessionId, auth.UserId);
        return ApiResponse.Ok(message: "Left session successfully.");
    }

    // ── START (instructor / admin only) ──────────────────────────────────────

    private static async Task<IResult> Start(
        long sessionId,
        IAuthContext auth,
        ISessionRepository sessionRepository,
        IUserRepository userRepository,
        IJaasTokenService jaasTokenService,
        IConfiguration configuration,
        INotificationService notificationService)
    {
        //if (!auth.IsAdminOrInstructor())
        //    return ApiResponse.Fail("This action is unauthorized.", StatusCodes.Status403Forbidden);

        await sessionRepository.StartAsync(sessionId);

        var session = await sessionRepository.FindAnyAsync(sessionId);
        if (session is null)
            return ApiResponse.Fail("Not found.", StatusCodes.Status404NotFound);

        await notificationService.NotifyUsersAsync(
            await SessionRecipientsAsync(sessionRepository, session, includeInstructor: false),
            "SessionStarted",
            "Session started",
            $"Session #{session.SessionNumber ?? session.Id} has started.",
            "video",
            new
            {
                session_id = session.Id,
                substance_category_id = session.SubstanceCategoryId
            });

        var user = await userRepository.FindAsync(auth.UserId);

        var (jwt, meetingUrl) = BuildJaasResponse(
            jaasTokenService,
            configuration,
            auth.UserId,
            user?.DisplayName ?? "Instructor",
            user?.AvatarUrl,
            session.JitsiRoomName,
            isModerator: true);

        return ApiResponse.Ok(new
        {
            session = SessionResource.From(session),
            jitsi_jwt = jwt,
            jitsi_room_name = session.JitsiRoomName,
            jitsi_server_url = GetServerUrl(configuration),
            meeting_url = meetingUrl,
            session_duration_minutes = session.DurationMinutes
        }, "Session started.");
    }

    // ── END ──────────────────────────────────────────────────────────────────

    private static async Task<IResult> End(
        long sessionId,
        IAuthContext auth,
        ISessionRepository sessionRepository)
    {
        if (!auth.IsAdminOrInstructor())
            return ApiResponse.Fail("This action is unauthorized.", StatusCodes.Status403Forbidden);

        await sessionRepository.EndAsync(sessionId);
        return ApiResponse.Ok(message: "Session ended.");
    }

    // ── FEEDBACK ─────────────────────────────────────────────────────────────

    private static async Task<IResult> Feedback(
        long sessionId,
        IAuthContext auth,
        HttpRequest httpRequest,
        ISessionRepository sessionRepository)
    {
        var payload = await RequestPayload.ReadAsync<FeedbackRequest>(httpRequest);
        if (payload.Error is not null)
            return payload.Error;

        var request = payload.Value!;

        if (request.rating is < 1 or > 5)
            return ApiResponse.Fail("Rating must be between 1 and 5.", StatusCodes.Status422UnprocessableEntity);

        var attendance = await sessionRepository.SaveFeedbackAsync(
            sessionId, auth.UserId, request.rating, request.comment);

        return attendance is null
            ? ApiResponse.Fail("You did not attend this session.", StatusCodes.Status404NotFound)
            : ApiResponse.Ok(
                new { attendance = AttendanceResource(attendance) },
                "Feedback submitted successfully.");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────────────────────────────────

    private static (string Jwt, string MeetingUrl) BuildJaasResponse(
        IJaasTokenService jaasTokenService,
        IConfiguration configuration,
        long userId,
        string displayName,
        string? avatarUrl,
        string roomName,
        bool isModerator)
    {
        var appId = configuration["Jaas:AppId"] ?? string.Empty;
        var serverUrl = GetServerUrl(configuration);

        try
        {
            var jwt = jaasTokenService.Create(userId, displayName, avatarUrl, roomName, isModerator);

            // JaaS meeting URL: https://8x8.vc/{appId}/{roomName}
            var meetingUrl = string.IsNullOrWhiteSpace(appId)
                ? $"{serverUrl}/{roomName}"
                : $"https://8x8.vc/{appId}/{roomName}";

            return (jwt, meetingUrl);
        }
        catch (InvalidOperationException ex)
        {
            Console.Error.WriteLine($"[JaaS] Token creation failed: {ex.Message}");
            return (string.Empty, string.Empty);
        }
    }

    private static string GetServerUrl(IConfiguration configuration)
        => configuration["Jaas:ServerUrl"]
           ?? configuration["Jitsi:ServerUrl"]
           ?? "https://8x8.vc";

    private static string? NormalizeSessionType(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "group";
        }

        var normalized = value.Trim().ToLowerInvariant();

        return normalized switch
        {
            "individual" or "single" or "one_to_one" or "one-to-one" or "فردي" or "فردية" => "individual",
            "group" or "جماعي" or "جماعية" => "group",
            _ => null
        };
    }

    private static object AttendanceResource(SessionAttendance attendance) => new
    {
        id = attendance.Id,
        session_id = attendance.SessionId,
        user_id = attendance.UserId,
        joined_at = EgyptDateTime.Format(attendance.JoinedAt),
        left_at = EgyptDateTime.Format(attendance.LeftAt),
        was_present = attendance.WasPresent,
        rating = attendance.Rating,
        comment = attendance.Comment,
        created_at = EgyptDateTime.Format(attendance.CreatedAt),
        updated_at = EgyptDateTime.Format(attendance.UpdatedAt)
    };

    private static async Task<IEnumerable<long>> SessionRecipientsAsync(
        ISessionRepository sessionRepository,
        TherapySession session,
        bool includeInstructor)
    {
        var memberIds = await sessionRepository.UserIdsForSubstanceCategoryAsync(session.SubstanceCategoryId);

        return includeInstructor
            ? memberIds.Append(session.InstructorId)
            : memberIds;
    }
}

public sealed record CreateSessionRequest(
    long? substance_category_id,
    long? group_id,
    long? substance_categories_id,
    long? instructor_id,
    int? session_number,
    string? title,
    string? session_type,
    DateTime scheduled_at,
    byte? duration_minutes,
    int? max_participants,
    object? metadata);

public sealed record FeedbackRequest(int rating, string? comment);
