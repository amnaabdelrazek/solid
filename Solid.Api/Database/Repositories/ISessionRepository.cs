using Solid.Api.Database.Entities;

namespace Solid.Api.Database.Repositories;

public interface ISessionRepository
{
    Task<IReadOnlyList<TherapySession>> ListForUserAsync(long userId, string? role);

    Task<IReadOnlyList<TherapySession>> UpcomingPaidForUserAsync(long userId);

    Task<IReadOnlyList<TherapySession>> UpcomingUnpaidForUserAsync(long userId);

    Task<TherapySession?> FindAsync(long sessionId, long userId, string? role);

    Task<TherapySession?> FindAnyAsync(long sessionId);

    Task<CreateSessionResult> CreateAsync(SessionCreate create);

    Task<JoinSessionResult> RecordJoinAsync(long sessionId, long userId);

    Task<SessionBookingResult> ValidateBookingAsync(long sessionId, long userId);

    Task LeaveAsync(long sessionId, long userId);

    Task StartAsync(long sessionId);

    Task EndAsync(long sessionId);

    Task<SessionAttendance?> SaveFeedbackAsync(long sessionId, long userId, int rating, string? comment);
}

public sealed record SessionCreate(
    long GroupId,
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
