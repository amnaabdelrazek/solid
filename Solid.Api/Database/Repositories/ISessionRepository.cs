using Solid.Api.Database.Entities;

namespace Solid.Api.Database.Repositories;

public interface ISessionRepository
{
    Task<IReadOnlyList<TherapySession>> ListForUserAsync(long userId);

    Task<IReadOnlyList<TherapySession>> PaidForUserAsync(long userId);

    Task<TherapySession?> FindAsync(long sessionId);

    Task RecordJoinAsync(long sessionId, long userId);

    Task LeaveAsync(long sessionId, long userId);

    Task StartAsync(long sessionId);

    Task EndAsync(long sessionId);

    Task<SessionAttendance?> SaveFeedbackAsync(long sessionId, long userId, int rating, string? comment);
}
