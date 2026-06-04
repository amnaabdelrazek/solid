using Microsoft.EntityFrameworkCore;
using Solid.Api.Database.Entities;

namespace Solid.Api.Database.Repositories;

public sealed class SessionRepository(SolidDbContext dbContext) : ISessionRepository
{
    public async Task<IReadOnlyList<TherapySession>> ListForUserAsync(long userId)
    {
        return await dbContext.TherapySessions
            .AsNoTracking()
            .Where(session => session.DeletedAt == null)
            .Where(session => session.Status != "finished")
            .Where(session =>
                session.InstructorId == userId ||
                session.Group.Members.Any(member => member.UserId == userId && member.IsActive))
            .OrderBy(session => session.SessionNumber)
            .ThenBy(session => session.ScheduledAt)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<TherapySession>> PaidForUserAsync(long userId)
    {
        return await dbContext.Payments
            .AsNoTracking()
            .Where(payment => payment.UserId == userId && payment.Status == "paid" && payment.Session != null && payment.Session.DeletedAt == null)
            .OrderBy(payment => payment.Session!.ScheduledAt)
            .Select(payment => payment.Session!)
            .Take(10)
            .ToListAsync();
    }

    public async Task<TherapySession?> FindAsync(long sessionId)
    {
        return await dbContext.TherapySessions
            .AsNoTracking()
            .FirstOrDefaultAsync(session => session.Id == sessionId && session.DeletedAt == null);
    }

    public async Task RecordJoinAsync(long sessionId, long userId)
    {
        var attendance = await dbContext.SessionAttendances
            .FirstOrDefaultAsync(attendance => attendance.SessionId == sessionId && attendance.UserId == userId);

        var now = DateTime.UtcNow;
        if (attendance is null)
        {
            dbContext.SessionAttendances.Add(new SessionAttendance
            {
                SessionId = sessionId,
                UserId = userId,
                JoinedAt = now,
                WasPresent = true,
                CreatedAt = now,
                UpdatedAt = now
            });
        }
        else
        {
            attendance.JoinedAt = now;
            attendance.WasPresent = true;
            attendance.UpdatedAt = now;
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task LeaveAsync(long sessionId, long userId)
    {
        var attendance = await dbContext.SessionAttendances
            .FirstOrDefaultAsync(attendance => attendance.SessionId == sessionId && attendance.UserId == userId && attendance.LeftAt == null);

        if (attendance is null)
        {
            return;
        }

        attendance.LeftAt = DateTime.UtcNow;
        attendance.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
    }

    public async Task StartAsync(long sessionId)
    {
        var session = await dbContext.TherapySessions.FirstOrDefaultAsync(session => session.Id == sessionId && session.DeletedAt == null);
        if (session is null)
        {
            return;
        }

        session.Status = "live";
        session.StartedAt = DateTime.UtcNow;
        session.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
    }

    public async Task EndAsync(long sessionId)
    {
        var session = await dbContext.TherapySessions.FirstOrDefaultAsync(session => session.Id == sessionId && session.DeletedAt == null);
        if (session is null)
        {
            return;
        }

        session.Status = "finished";
        session.EndedAt = DateTime.UtcNow;
        session.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
    }

    public async Task<SessionAttendance?> SaveFeedbackAsync(long sessionId, long userId, int rating, string? comment)
    {
        var attendance = await dbContext.SessionAttendances
            .FirstOrDefaultAsync(attendance => attendance.SessionId == sessionId && attendance.UserId == userId);

        if (attendance is null)
        {
            return null;
        }

        attendance.Rating = (byte)rating;
        attendance.Comment = comment;
        attendance.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        return attendance;
    }
}
