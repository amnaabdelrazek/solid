using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Solid.Api.Database.Entities;

namespace Solid.Api.Database.Repositories;

public sealed class SessionRepository(SolidDbContext dbContext) : ISessionRepository
{
    public async Task<IReadOnlyList<TherapySession>> ListForUserAsync(long userId, string? role)
    {
        var query = ActiveSessionsQuery();

        if (string.Equals(role, "admin", StringComparison.OrdinalIgnoreCase))
        {
            return await query
                .OrderBy(session => session.ScheduledAt)
                .ToListAsync();
        }

        if (string.Equals(role, "instructor", StringComparison.OrdinalIgnoreCase))
        {
            return await query
                .Where(session => session.InstructorId == userId)
                .OrderBy(session => session.ScheduledAt)
                .ToListAsync();
        }

        var sessions = await query
            .OrderBy(session => session.ScheduledAt)
            .ToListAsync();

        return sessions
            .Where(session => !IsFull(session))
            .ToList();
    }

    public async Task<IReadOnlyList<TherapySession>> UpcomingPaidForUserAsync(long userId)
    {
        var now = DateTime.UtcNow;

        return await dbContext.TherapySessions
            .AsNoTracking()
            .Include(session => session.Group)
            .Include(session => session.Attendances)
            .Include(session => session.Instructor)
            .Where(session => session.DeletedAt == null)
            .Where(session => session.ScheduledAt > now)
            .Where(session => session.Payments.Any(payment => payment.UserId == userId && payment.Status == "paid"))
            .OrderBy(session => session.SessionNumber ?? int.MaxValue)
            .ThenBy(session => session.ScheduledAt)
            .Take(10)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<TherapySession>> UpcomingUnpaidForUserAsync(long userId)
    {
        var now = DateTime.UtcNow;

        return await dbContext.TherapySessions
            .AsNoTracking()
            .Include(session => session.Group)
            .Include(session => session.Attendances)
            .Include(session => session.Instructor)
            .Where(session => session.DeletedAt == null)
            .Where(session => session.ScheduledAt > now)
            .Where(session => session.Group.Members.Any(member => member.UserId == userId && member.IsActive))
            .Where(session => !session.Payments.Any(payment => payment.UserId == userId && payment.Status == "paid"))
            .OrderBy(session => session.SessionNumber ?? int.MaxValue)
            .ThenBy(session => session.ScheduledAt)
            .Take(10)
            .ToListAsync();
    }

    public async Task<TherapySession?> FindAsync(long sessionId, long userId, string? role)
    {
        var session = await ActiveSessionsQuery()
            .FirstOrDefaultAsync(session => session.Id == sessionId);

        if (session is null)
        {
            return null;
        }

        if (string.Equals(role, "admin", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(role, "instructor", StringComparison.OrdinalIgnoreCase) && session.InstructorId == userId ||
            session.Group.Members.Any(member => member.UserId == userId && member.IsActive))
        {
            return session;
        }

        return null;
    }

    public async Task<TherapySession?> FindAnyAsync(long sessionId)
    {
        return await dbContext.TherapySessions
            .AsNoTracking()
            .Include(session => session.Group.Members)
            .Include(session => session.Attendances)
            .FirstOrDefaultAsync(session => session.Id == sessionId && session.DeletedAt == null);
    }

    public async Task<CreateSessionResult> CreateAsync(SessionCreate create)
    {
        var group = await dbContext.Groups
            .FirstOrDefaultAsync(group => group.Id == create.GroupId && group.DeletedAt == null);

        if (group is null)
        {
            return new CreateSessionResult(null, "Group not found.", StatusCodes.Status404NotFound);
        }

        var instructor = await dbContext.Users
            .FirstOrDefaultAsync(user => user.Id == create.InstructorId && user.Role == "instructor" && user.DeletedAt == null);

        if (instructor is null)
        {
            return new CreateSessionResult(null, "Instructor not found.", StatusCodes.Status404NotFound);
        }

        var nextSessionNumber = create.SessionNumber ?? await dbContext.TherapySessions
            .Where(session => session.GroupId == create.GroupId)
            .Select(session => session.SessionNumber ?? 0)
            .DefaultIfEmpty()
            .MaxAsync() + 1;

        var now = DateTime.UtcNow;
        var session = new TherapySession
        {
            GroupId = create.GroupId,
            InstructorId = create.InstructorId,
            SessionNumber = nextSessionNumber,
            SessionType = create.SessionType,
            Status = "scheduled",
            ScheduledAt = create.ScheduledAt,
            DurationMinutes = create.DurationMinutes,
            JitsiRoomName = $"solid-group-{create.GroupId}-{Guid.NewGuid():N}",
            SessionMetadata = BuildMetadata(create),
            CreatedAt = now,
            UpdatedAt = now
        };

        group.InstructorId ??= create.InstructorId;
        group.UpdatedAt = now;

        dbContext.TherapySessions.Add(session);
        await dbContext.SaveChangesAsync();

        return new CreateSessionResult(
            await FindAnyAsync(session.Id),
            null,
            StatusCodes.Status200OK);
    }

    public async Task<JoinSessionResult> RecordJoinAsync(long sessionId, long userId)
    {
        var session = await dbContext.TherapySessions
            .Include(session => session.Group.Members)
            .Include(session => session.Attendances)
            .FirstOrDefaultAsync(session => session.Id == sessionId && session.DeletedAt == null);

        if (session is null)
        {
            return new JoinSessionResult(false, null, "Not found.", StatusCodes.Status404NotFound);
        }

        if (session.Status != "live")
        {
            return new JoinSessionResult(false, session, "Session is not live.", StatusCodes.Status422UnprocessableEntity);
        }

        if (!session.Group.Members.Any(member => member.UserId == userId && member.IsActive))
        {
            return new JoinSessionResult(false, session, "You are not subscribed to this session group.", StatusCodes.Status403Forbidden);
        }

        if (IsFull(session) && session.Attendances.All(attendance => attendance.UserId != userId))
        {
            return new JoinSessionResult(false, session, "Session is full.", StatusCodes.Status422UnprocessableEntity);
        }

        var attendance = session.Attendances.FirstOrDefault(attendance => attendance.UserId == userId);
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

        return new JoinSessionResult(true, session, null, StatusCodes.Status200OK);
    }

    public async Task<SessionBookingResult> ValidateBookingAsync(long sessionId, long userId)
    {
        var session = await dbContext.TherapySessions
            .AsNoTracking()
            .Include(session => session.Group.Members)
            .FirstOrDefaultAsync(session => session.Id == sessionId && session.DeletedAt == null);

        if (session is null)
        {
            return new SessionBookingResult(false, "Session not found.", StatusCodes.Status404NotFound);
        }

        if (session.ScheduledAt <= DateTime.UtcNow)
        {
            return new SessionBookingResult(false, "You can only book upcoming sessions.", StatusCodes.Status422UnprocessableEntity);
        }

        if (!session.Group.Members.Any(member => member.UserId == userId && member.IsActive))
        {
            return new SessionBookingResult(false, "You are not subscribed to this session group.", StatusCodes.Status403Forbidden);
        }

        var alreadyPaid = await dbContext.Payments
            .AsNoTracking()
            .AnyAsync(payment => payment.UserId == userId && payment.SessionId == sessionId && payment.Status == "paid");

        if (alreadyPaid)
        {
            return new SessionBookingResult(false, "You have already paid for this session.", StatusCodes.Status422UnprocessableEntity);
        }

        var now = DateTime.UtcNow;
        var previousPaidSessions = dbContext.TherapySessions
            .AsNoTracking()
            .Where(previous => previous.DeletedAt == null)
            .Where(previous => previous.GroupId == session.GroupId)
            .Where(previous => previous.ScheduledAt > now)
            .Where(previous => previous.Id != session.Id);

        previousPaidSessions = session.SessionNumber.HasValue
            ? previousPaidSessions.Where(previous =>
                previous.SessionNumber.HasValue && previous.SessionNumber.Value < session.SessionNumber.Value ||
                previous.SessionNumber == session.SessionNumber && (previous.ScheduledAt < session.ScheduledAt ||
                    previous.ScheduledAt == session.ScheduledAt && previous.Id < session.Id))
            : previousPaidSessions.Where(previous => previous.ScheduledAt < session.ScheduledAt ||
                previous.ScheduledAt == session.ScheduledAt && previous.Id < session.Id);

        var firstUnpaidPreviousSession = await previousPaidSessions
            .Where(previous => !previous.Payments.Any(payment => payment.UserId == userId && payment.Status == "paid"))
            .OrderBy(previous => previous.SessionNumber ?? int.MaxValue)
            .ThenBy(previous => previous.ScheduledAt)
            .FirstOrDefaultAsync();

        if (firstUnpaidPreviousSession is not null)
        {
            return new SessionBookingResult(
                false,
                $"You must book session #{firstUnpaidPreviousSession.SessionNumber ?? firstUnpaidPreviousSession.Id} first.",
                StatusCodes.Status422UnprocessableEntity);
        }

        return new SessionBookingResult(true, null, StatusCodes.Status200OK);
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

    private IQueryable<TherapySession> ActiveSessionsQuery()
    {
        return dbContext.TherapySessions
            .AsNoTracking()
            .Include(session => session.Group.Members)
            .Include(session => session.Attendances)
            .Where(session => session.DeletedAt == null)
            /*.Where(session => session.Status != "finished")*/;
    }

    private static bool IsFull(TherapySession session)
    {
        return session.Attendances.Count(attendance => attendance.WasPresent) >= MaxParticipants(session);
    }

    private static int MaxParticipants(TherapySession session)
    {
        var metadata = ReadMetadata(session.SessionMetadata);

        if (metadata.TryGetValue("max_participants", out var value) &&
            int.TryParse(Convert.ToString(value), out var maxParticipants) &&
            maxParticipants > 0)
        {
            return maxParticipants;
        }

        return session.Group.MaxMembers;
    }

    private static string BuildMetadata(SessionCreate create)
    {
        var metadata = ReadMetadata(create.Metadata);

        if (!string.IsNullOrWhiteSpace(create.Title))
        {
            metadata["title"] = create.Title;
        }

        if (create.MaxParticipants is > 0)
        {
            metadata["max_participants"] = create.MaxParticipants;
        }

        return JsonSerializer.Serialize(metadata);
    }

    private static Dictionary<string, object?> ReadMetadata(object? value)
    {
        if (value is null)
        {
            return [];
        }

        if (value is Dictionary<string, object?> dictionary)
        {
            return dictionary;
        }

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, object?>>(Convert.ToString(value) ?? string.Empty) ?? [];
        }
        catch
        {
            return [];
        }
    }
}
