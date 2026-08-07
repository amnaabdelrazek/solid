using Solid.Api.Database.Entities;

namespace Solid.Api.Database.Repositories;

public interface ISessionRepository
{
    Task<IReadOnlyList<TherapySession>> ListForUserAsync(long userId, string? role);

    Task<(IReadOnlyList<TherapySession> Sessions, int TotalCount)> ListForUserPaginatedAsync(long userId, string? role, int page, int pageSize);

    Task<IReadOnlyList<TherapySession>> UpcomingPaidForUserAsync(long userId, string? sessionType = null);
    Task<(IReadOnlyList<TherapySession> Sessions, int TotalCount)> UpcomingPaidForUserPaginatedAsync(long userId, string? sessionType = null, int page = 1, int pageSize = 10);

    Task<IReadOnlyList<TherapySession>> AttendedSessionsForUserAsync(long userId);
    Task<(IReadOnlyList<TherapySession> Sessions, int TotalCount)> AttendedSessionsForUserPaginatedAsync(long userId, int page, int pageSize);

    Task<TherapySession?> FindWithAttendeesAsync(long sessionId);

    Task<IReadOnlyList<TherapySession>> UpcomingUnpaidForUserAsync(long userId, string? sessionType = null);
    Task<(IReadOnlyList<TherapySession> Sessions, int TotalCount)> UpcomingUnpaidForUserPaginatedAsync(long userId, string? sessionType = null, int page = 1, int pageSize = 10);

    Task<TherapySession?> FindAsync(long sessionId, long userId, string? role);

    Task<TherapySession?> FindAnyAsync(long sessionId);

    Task<CreateSessionResult> CreateAsync(SessionCreate create);

    Task<IReadOnlyList<long>> UserIdsForSubstanceCategoryAsync(long substanceCategoryId);

    Task<JoinSessionResult> RecordJoinAsync(long sessionId, long userId);

    Task<SessionBookingResult> ValidateBookingAsync(long sessionId, long userId);

    Task LeaveAsync(long sessionId, long userId);

    Task StartAsync(long sessionId);

    Task EndAsync(long sessionId);

    Task<SessionAttendance?> SaveFeedbackAsync(long sessionId, long userId, int rating, string? comment);
}

public sealed record SessionCreate(
    long SubstanceCategoryId,
    long InstructorId,
    int? SessionNumber,
    string? Title,
    string SessionType,
    DateTime ScheduledAt,
    byte DurationMinutes,
    int? MaxParticipants,
    object? Metadata);

public sealed record CreateSessionResult(TherapySession? Session, string? Error, int StatusCode);

public sealed record JoinSessionResult(bool Success, TherapySession? Session, string? Error, int StatusCode);

public sealed record SessionBookingResult(bool Success, string? Error, int StatusCode);
