namespace Solid.Api.Database.Repositories;

public interface ISessionRepository
{
    Task<IReadOnlyList<Dictionary<string, object?>>> ListForUserAsync(long userId);

    Task<IReadOnlyList<Dictionary<string, object?>>> PaidForUserAsync(long userId);

    Task<Dictionary<string, object?>?> FindAsync(long sessionId);

    Task RecordJoinAsync(long sessionId, long userId);

    Task LeaveAsync(long sessionId, long userId);

    Task StartAsync(long sessionId);

    Task EndAsync(long sessionId);

    Task<Dictionary<string, object?>?> SaveFeedbackAsync(long sessionId, long userId, int rating, string? comment);
}
