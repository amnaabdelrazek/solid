using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Solid.Api.Database.Entities;

namespace Solid.Api.Database.Repositories;

public sealed class SessionRepository(SolidDbContext dbContext) : ISessionRepository
{
    private const string IndividualSessionType = "individual";
    private const int MaxSessionParticipants = 15;

    public async Task<IReadOnlyList<TherapySession>> ListForUserAsync(long userId, string? role)
    {
        var query = ActiveSessionsQuery();

        if (string.Equals(role, "admin", StringComparison.OrdinalIgnoreCase))
        {
            // Admin: every session in the system.
            return await query
                .OrderBy(session => session.ScheduledAt)
                .ToListAsync();
        }

        if (string.Equals(role, "instructor", StringComparison.OrdinalIgnoreCase))
        {
            // Instructor: only the sessions they are assigned to teach.
            return await query
                .Where(session => session.InstructorId == userId)
                .OrderBy(session => session.ScheduledAt)
                .ToListAsync();
        }

        // Regular user (addict): only sessions they have paid for or already attended.
        return await query
            .Where(session =>
                session.Payments.Any(payment => payment.UserId == userId && payment.Status == "paid") ||
                session.Attendances.Any(attendance => attendance.UserId == userId && attendance.WasPresent))
            .OrderBy(session => session.ScheduledAt)
            .ToListAsync();
    }

    public async Task<(IReadOnlyList<TherapySession> Sessions, int TotalCount)> ListForUserPaginatedAsync(long userId, string? role, int page, int pageSize)
    {
        var query = ActiveSessionsQuery();

        if (string.Equals(role, "admin", StringComparison.OrdinalIgnoreCase))
        {
            // Admin: every session in the system.
            query = query.Where(session => true);
        }
        else if (string.Equals(role, "instructor", StringComparison.OrdinalIgnoreCase))
        {
            // Instructor: only the sessions they are assigned to teach.
            query = query.Where(session => session.InstructorId == userId);
        }
        else
        {
            // Regular user (addict): only sessions they have paid for or already attended.
            query = query.Where(session =>
                session.Payments.Any(payment => payment.UserId == userId && payment.Status == "paid") ||
                session.Attendances.Any(attendance => attendance.UserId == userId && attendance.WasPresent));
        }

        var totalCount = await query.CountAsync();

        var sessions = await query
            .OrderBy(session => session.ScheduledAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (sessions, totalCount);
    }

    public async Task<IReadOnlyList<TherapySession>> UpcomingPaidForUserAsync(long userId, string? sessionType = null)
    {
        var now = DateTime.UtcNow;

        var query = dbContext.TherapySessions
            .AsNoTracking()
            .Include(session => session.SubstanceCategory)
            .Include(session => session.Attendances)
            .Include(session => session.Payments)
            .Include(session => session.Instructor)
            .Where(session => session.DeletedAt == null)
            .Where(session => session.ScheduledAt > now)
            .Where(session => session.Payments.Any(payment => payment.UserId == userId && payment.Status == "paid"));

        if (!string.IsNullOrWhiteSpace(sessionType))
        {
            query = query.Where(session => session.SessionType == sessionType);
        }

        return await query
            .OrderBy(session => session.SessionNumber ?? int.MaxValue)
            .ThenBy(session => session.ScheduledAt)
            .Take(10)
            .ToListAsync();
    }

    public async Task<(IReadOnlyList<TherapySession> Sessions, int TotalCount)> UpcomingPaidForUserPaginatedAsync(long userId, string? sessionType = null, int page = 1, int pageSize = 10)
    {
        var now = DateTime.UtcNow;

        var query = dbContext.TherapySessions
            .AsNoTracking()
            .Include(session => session.SubstanceCategory)
            .Include(session => session.Attendances)
            .Include(session => session.Payments)
            .Include(session => session.Instructor)
            .Where(session => session.DeletedAt == null)
            .Where(session => session.ScheduledAt > now)
            .Where(session => session.Payments.Any(payment => payment.UserId == userId && payment.Status == "paid"));

        if (!string.IsNullOrWhiteSpace(sessionType))
        {
            query = query.Where(session => session.SessionType == sessionType);
        }

        var totalCount = await query.CountAsync();

        var sessions = await query
            .OrderBy(session => session.SessionNumber ?? int.MaxValue)
            .ThenBy(session => session.ScheduledAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (sessions, totalCount);
    }

    //public async Task<IReadOnlyList<TherapySession>> UpcomingUnpaidForUserAsync(long userId)
    //{
    //    var now = DateTime.UtcNow;

    //    return await dbContext.TherapySessions
    //        .AsNoTracking()
    //        .Include(session => session.SubstanceCategory)
    //        .Include(session => session.Attendances)
    //        .Include(session => session.Payments)
    //        .Include(session => session.Instructor)
    //        .Where(session => session.DeletedAt == null)
    //        .Where(session => session.ScheduledAt > now)
    //        .Where(session => dbContext.UserSubstances.Any(userSubstance =>
    //            userSubstance.UserId == userId &&
    //            userSubstance.Substance.SubstanceCategoryId == session.SubstanceCategoryId))
    //        .Where(session => !session.Payments.Any(payment => payment.UserId == userId && payment.Status == "paid"))
    //        .Where(session => session.Payments.Count(payment => payment.Status == "paid") < MaxSessionParticipants)
    //        .OrderBy(session => session.SessionNumber ?? int.MaxValue)
    //        .ThenBy(session => session.ScheduledAt)
    //        .Take(10)
    //        .ToListAsync();
    //}


    public async Task<IReadOnlyList<TherapySession>> UpcomingUnpaidForUserAsync(long userId, string? sessionType = null)
    {
        var now = DateTime.UtcNow;

        var query = dbContext.TherapySessions
            .AsNoTracking()
            .Include(session => session.SubstanceCategory)
            .Include(session => session.Attendances)
            .Include(session => session.Payments)
            .Include(session => session.Instructor)
            .Where(session => session.DeletedAt == null)
            .Where(session => session.ScheduledAt > now)
            .Where(session => dbContext.UserSubstances.Any(userSubstance =>
                userSubstance.UserId == userId &&
                userSubstance.Substance.SubstanceCategoryId == session.SubstanceCategoryId))
            .Where(session => !session.Payments.Any(payment =>
                payment.UserId == userId &&
                payment.Status == "paid"));

        if (!string.IsNullOrWhiteSpace(sessionType))
        {
            query = query.Where(session => session.SessionType == sessionType);
        }

        var sessions = await query
            .OrderBy(session => session.SessionNumber ?? int.MaxValue)
            .ThenBy(session => session.ScheduledAt)
            .ToListAsync();

        if (sessions.Count == 0)
        {
            return sessions;
        }

        // Show only the session number that is due after the user's last attended session.
        var categoryIds = sessions.Select(session => session.SubstanceCategoryId).Distinct().ToList();
        var nextSessionNumberByCategory = new Dictionary<long, int>();

        foreach (var categoryId in categoryIds)
        {
            var lastAttendedSessionNumber = await dbContext.SessionAttendances
                .AsNoTracking()
                .Where(attendance => attendance.UserId == userId && attendance.WasPresent)
                .Where(attendance =>
                    attendance.Session.DeletedAt == null &&
                    attendance.Session.SubstanceCategoryId == categoryId &&
                    attendance.Session.SessionNumber.HasValue)
                .Select(attendance => (int?)attendance.Session.SessionNumber)
                .MaxAsync() ?? 0;

            var lastPaidSessionNumber = await dbContext.Payments
                .AsNoTracking()
                .Where(payment => payment.UserId == userId && payment.Status == "paid" && payment.Session != null)
                .Where(payment =>
                    payment.Session!.DeletedAt == null &&
                    payment.Session!.SubstanceCategoryId == categoryId &&
                    payment.Session!.SessionNumber.HasValue)
                .Select(payment => (int?)payment.Session!.SessionNumber)
                .MaxAsync() ?? 0;

            var lastSessionNumber = Math.Max(lastAttendedSessionNumber, lastPaidSessionNumber);
            nextSessionNumberByCategory[categoryId] = lastSessionNumber + 1;
        }

        var visibleSessions = new List<TherapySession>();

        foreach (var session in sessions)
        {
            // Unnumbered sessions are not part of the sequence, so keep them visible.
            if (!session.SessionNumber.HasValue)
            {
                visibleSessions.Add(session);
                continue;
            }

            var nextSessionNumber = nextSessionNumberByCategory.GetValueOrDefault(session.SubstanceCategoryId, 1);

            if (session.SessionNumber.Value <= nextSessionNumber)
            {
                visibleSessions.Add(session);
            }
        }

        return visibleSessions
            .OrderBy(session => session.SessionNumber ?? int.MaxValue)
            .ThenBy(session => session.ScheduledAt)
            .Take(10)
            .ToList();
    }

    public async Task<(IReadOnlyList<TherapySession> Sessions, int TotalCount)> UpcomingUnpaidForUserPaginatedAsync(long userId, string? sessionType = null, int page = 1, int pageSize = 10)
    {
        var now = DateTime.UtcNow;

        var query = dbContext.TherapySessions
            .AsNoTracking()
            .Include(session => session.SubstanceCategory)
            .Include(session => session.Attendances)
            .Include(session => session.Payments)
            .Include(session => session.Instructor)
            .Where(session => session.DeletedAt == null)
            .Where(session => session.ScheduledAt > now)
            .Where(session => dbContext.UserSubstances.Any(userSubstance =>
                userSubstance.UserId == userId &&
                userSubstance.Substance.SubstanceCategoryId == session.SubstanceCategoryId))
            .Where(session => !session.Payments.Any(payment =>
                payment.UserId == userId &&
                payment.Status == "paid"));

        if (!string.IsNullOrWhiteSpace(sessionType))
        {
            query = query.Where(session => session.SessionType == sessionType);
        }

        var sessions = await query
            .OrderBy(session => session.SessionNumber ?? int.MaxValue)
            .ThenBy(session => session.ScheduledAt)
            .ToListAsync();

        if (sessions.Count == 0)
        {
            return (sessions, 0);
        }

        var categoryIds = sessions.Select(session => session.SubstanceCategoryId).Distinct().ToList();
        var nextSessionNumberByCategory = new Dictionary<long, int>();

        foreach (var categoryId in categoryIds)
        {
            var lastAttendedSessionNumber = await dbContext.SessionAttendances
                .AsNoTracking()
                .Where(attendance => attendance.UserId == userId && attendance.WasPresent)
                .Where(attendance =>
                    attendance.Session.DeletedAt == null &&
                    attendance.Session.SubstanceCategoryId == categoryId &&
                    attendance.Session.SessionNumber.HasValue)
                .Select(attendance => (int?)attendance.Session.SessionNumber)
                .MaxAsync() ?? 0;

            var lastPaidSessionNumber = await dbContext.Payments
                .AsNoTracking()
                .Where(payment => payment.UserId == userId && payment.Status == "paid" && payment.Session != null)
                .Where(payment =>
                    payment.Session!.DeletedAt == null &&
                    payment.Session!.SubstanceCategoryId == categoryId &&
                    payment.Session!.SessionNumber.HasValue)
                .Select(payment => (int?)payment.Session!.SessionNumber)
                .MaxAsync() ?? 0;

            var lastSessionNumber = Math.Max(lastAttendedSessionNumber, lastPaidSessionNumber);
            nextSessionNumberByCategory[categoryId] = lastSessionNumber + 1;
        }

        var visibleSessions = new List<TherapySession>();

        foreach (var session in sessions)
        {
            if (!session.SessionNumber.HasValue)
            {
                visibleSessions.Add(session);
                continue;
            }

            var nextSessionNumber = nextSessionNumberByCategory.GetValueOrDefault(session.SubstanceCategoryId, 1);

            if (session.SessionNumber.Value <= nextSessionNumber)
            {
                visibleSessions.Add(session);
            }
        }

        var totalCount = visibleSessions.Count;
        var paginatedSessions = visibleSessions
            .OrderBy(session => session.SessionNumber ?? int.MaxValue)
            .ThenBy(session => session.ScheduledAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return (paginatedSessions, totalCount);
    }

    public async Task<TherapySession?> FindWithAttendeesAsync(long sessionId)
    {
        return await dbContext.TherapySessions
            .AsNoTracking()
            .Include(session => session.SubstanceCategory)
            .Include(session => session.Instructor)
            .Include(session => session.Attendances)
                .ThenInclude(attendance => attendance.User)
            .FirstOrDefaultAsync(session => session.Id == sessionId && session.DeletedAt == null);
    }

    public async Task<IReadOnlyList<TherapySession>> AttendedSessionsForUserAsync(long userId)
    {
        return await dbContext.TherapySessions
            .AsNoTracking()
            .Include(session => session.SubstanceCategory)
            .Include(session => session.Instructor)
            .Include(session => session.Attendances)
            .Include(session => session.Payments)
            .Where(session => session.DeletedAt == null)
            .Where(session => session.Attendances.Any(attendance =>
                attendance.UserId == userId && attendance.WasPresent))
            .OrderByDescending(session => session.ScheduledAt)
            .ToListAsync();
    }

    public async Task<(IReadOnlyList<TherapySession> Sessions, int TotalCount)> AttendedSessionsForUserPaginatedAsync(long userId, int page, int pageSize)
    {
        var query = dbContext.TherapySessions
            .AsNoTracking()
            .Include(session => session.SubstanceCategory)
            .Include(session => session.Instructor)
            .Include(session => session.Attendances)
            .Include(session => session.Payments)
            .Where(session => session.DeletedAt == null)
            .Where(session => session.Attendances.Any(attendance =>
                attendance.UserId == userId && attendance.WasPresent));

        var totalCount = await query.CountAsync();

        var sessions = await query
            .OrderByDescending(session => session.ScheduledAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (sessions, totalCount);
    }


    public async Task<TherapySession?> FindAsync(long sessionId, long userId, string? role)
    {
        await ExpireLiveSessionsAsync();

        var session = await ActiveSessionsQuery()

            .FirstOrDefaultAsync(session => session.Id == sessionId);

        if (session is null)
        {
            return null;
        }

        if (string.Equals(role, "admin", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(role, "instructor", StringComparison.OrdinalIgnoreCase) && session.InstructorId == userId ||
            await UserHasSubstanceCategoryAsync(userId, session.SubstanceCategoryId))
        {
            return session;
        }

        return null;
    }

    public async Task<TherapySession?> FindAnyAsync(long sessionId)
    {
        await ExpireLiveSessionsAsync();

        return await dbContext.TherapySessions
            .AsNoTracking()
            .Include(session => session.SubstanceCategory)
            .Include(session => session.Attendances)
            .Include(session => session.Payments)
            .FirstOrDefaultAsync(session => session.Id == sessionId && session.DeletedAt == null);
    }

    public async Task<CreateSessionResult> CreateAsync(SessionCreate create)
    {
        var substanceCategory = await dbContext.SubstanceCategories
            .FirstOrDefaultAsync(category => category.Id == create.SubstanceCategoryId && category.IsActive);

        if (substanceCategory is null)
        {
            return new CreateSessionResult(null, "Substance category not found.", StatusCodes.Status404NotFound);
        }

        var instructor = await dbContext.Users
            .FirstOrDefaultAsync(user => user.Id == create.InstructorId && user.Role == "instructor" && user.DeletedAt == null);

        if (instructor is null)
        {
            return new CreateSessionResult(null, "Instructor not found.", StatusCodes.Status404NotFound);
        }

        var nextSessionNumber = create.SessionNumber ?? await dbContext.TherapySessions
            .Where(session => session.SubstanceCategoryId == create.SubstanceCategoryId)
            .Select(session => session.SessionNumber ?? 0)
            .DefaultIfEmpty()
            .MaxAsync() + 1;

        var now = DateTime.UtcNow;
        var session = new TherapySession
        {
            SubstanceCategoryId = create.SubstanceCategoryId,
            InstructorId = create.InstructorId,
            SessionNumber = nextSessionNumber,
            SessionType = create.SessionType,
            Status = "scheduled",
            ScheduledAt = create.ScheduledAt,
            DurationMinutes = create.DurationMinutes,
            JitsiRoomName = $"solid-category-{create.SubstanceCategoryId}-{Guid.NewGuid():N}",
            SessionMetadata = BuildMetadata(create),
            CreatedAt = now,
            UpdatedAt = now
        };

        dbContext.TherapySessions.Add(session);
        await dbContext.SaveChangesAsync();

        return new CreateSessionResult(
            await FindAnyAsync(session.Id),
            null,
            StatusCodes.Status200OK);
    }

    public async Task<IReadOnlyList<long>> UserIdsForSubstanceCategoryAsync(long substanceCategoryId)
    {
        return await dbContext.UserSubstances
            .AsNoTracking()
            .Where(userSubstance => userSubstance.Substance.SubstanceCategoryId == substanceCategoryId)
            .Select(userSubstance => userSubstance.UserId)
            .Distinct()
            .ToListAsync();
    }

    public async Task<JoinSessionResult> RecordJoinAsync(long sessionId, long userId)
    {
        await ExpireLiveSessionsAsync();

        var session = await dbContext.TherapySessions
            .Include(session => session.SubstanceCategory)
            .Include(session => session.Attendances)
            .Include(session => session.Payments)
            .FirstOrDefaultAsync(session => session.Id == sessionId && session.DeletedAt == null);

        if (session is null)
        {
            return new JoinSessionResult(false, null, "Not found.", StatusCodes.Status404NotFound);
        }

        if (session.Status != "live")
        {
            return new JoinSessionResult(false, session, "Session is not live.", StatusCodes.Status422UnprocessableEntity);
        }

        if (!await UserHasSubstanceCategoryAsync(userId, session.SubstanceCategoryId))
        {
            return new JoinSessionResult(false, session, "You are not subscribed to this session category.", StatusCodes.Status403Forbidden);
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

    //public async Task<SessionBookingResult> ValidateBookingAsync(long sessionId, long userId)
    //{
    //    var session = await dbContext.TherapySessions
    //        .AsNoTracking()
    //        .Include(session => session.SubstanceCategory)
    //        .FirstOrDefaultAsync(session => session.Id == sessionId && session.DeletedAt == null);

    //    if (session is null)
    //    {
    //        return new SessionBookingResult(false, "Session not found.", StatusCodes.Status404NotFound);
    //    }

    //    if (session.ScheduledAt <= DateTime.UtcNow)
    //    {
    //        return new SessionBookingResult(false, "You can only book upcoming sessions.", StatusCodes.Status422UnprocessableEntity);
    //    }

    //    if (!await UserHasSubstanceCategoryAsync(userId, session.SubstanceCategoryId))
    //    {
    //        return new SessionBookingResult(false, "You are not subscribed to this session category.", StatusCodes.Status403Forbidden);
    //    }

    //    var paidRegistrations = await dbContext.Payments
    //        .AsNoTracking()
    //        .CountAsync(payment => payment.SessionId == sessionId && payment.Status == "paid");

    //    if (paidRegistrations >= MaxParticipants(session))
    //    {
    //        return new SessionBookingResult(false, "Session is full.", StatusCodes.Status422UnprocessableEntity);
    //    }

    //    var alreadyPaid = await dbContext.Payments
    //        .AsNoTracking()
    //        .AnyAsync(payment => payment.UserId == userId && payment.SessionId == sessionId && payment.Status == "paid");

    //    if (alreadyPaid)
    //    {
    //        return new SessionBookingResult(false, "You have already paid for this session.", StatusCodes.Status422UnprocessableEntity);
    //    }

    //    var now = DateTime.UtcNow;
    //    var previousPaidSessions = dbContext.TherapySessions
    //        .AsNoTracking()
    //        .Where(previous => previous.DeletedAt == null)
    //        .Where(previous => previous.SubstanceCategoryId == session.SubstanceCategoryId)
    //        .Where(previous => previous.ScheduledAt > now)
    //        .Where(previous => previous.Id != session.Id);

    //    previousPaidSessions = session.SessionNumber.HasValue
    //        ? previousPaidSessions.Where(previous =>
    //            previous.SessionNumber.HasValue && previous.SessionNumber.Value < session.SessionNumber.Value ||
    //            previous.SessionNumber == session.SessionNumber && (previous.ScheduledAt < session.ScheduledAt ||
    //                previous.ScheduledAt == session.ScheduledAt && previous.Id < session.Id))
    //        : previousPaidSessions.Where(previous => previous.ScheduledAt < session.ScheduledAt ||
    //            previous.ScheduledAt == session.ScheduledAt && previous.Id < session.Id);

    //    var firstUnpaidPreviousSession = await previousPaidSessions
    //        .Where(previous => !previous.Payments.Any(payment => payment.UserId == userId && payment.Status == "paid"))
    //        .OrderBy(previous => previous.SessionNumber ?? int.MaxValue)
    //        .ThenBy(previous => previous.ScheduledAt)
    //        .FirstOrDefaultAsync();

    //    if (firstUnpaidPreviousSession is not null)
    //    {
    //        return new SessionBookingResult(
    //            false,
    //            $"You must book session #{firstUnpaidPreviousSession.SessionNumber ?? firstUnpaidPreviousSession.Id} first.",
    //            StatusCodes.Status422UnprocessableEntity);
    //    }

    //    return new SessionBookingResult(true, null, StatusCodes.Status200OK);
    //}



    public async Task<SessionBookingResult> ValidateBookingAsync(long sessionId, long userId)
    {
        var session = await dbContext.TherapySessions
            .AsNoTracking()
            .Include(session => session.SubstanceCategory)
            .FirstOrDefaultAsync(session => session.Id == sessionId && session.DeletedAt == null);

        if (session is null)
        {
            return new SessionBookingResult(false, "Session not found.", StatusCodes.Status404NotFound);
        }

        if (session.ScheduledAt <= DateTime.UtcNow)
        {
            return new SessionBookingResult(false, "You can only book upcoming sessions.", StatusCodes.Status422UnprocessableEntity);
        }

        if (!await UserHasSubstanceCategoryAsync(userId, session.SubstanceCategoryId))
        {
            return new SessionBookingResult(false, "You are not subscribed to this session category.", StatusCodes.Status403Forbidden);
        }

        var paidRegistrations = await dbContext.Payments
            .AsNoTracking()
            .CountAsync(payment =>
                payment.SessionId == sessionId &&
                payment.Status == "paid");

        if (paidRegistrations >= MaxParticipants(session))
        {
            return new SessionBookingResult(false, "Session is full.", StatusCodes.Status422UnprocessableEntity);
        }

        var alreadyPaid = await dbContext.Payments
            .AsNoTracking()
            .AnyAsync(payment =>
                payment.UserId == userId &&
                payment.SessionId == sessionId &&
                payment.Status == "paid");

        if (alreadyPaid)
        {
            return new SessionBookingResult(false, "You have already paid for this session.", StatusCodes.Status422UnprocessableEntity);
        }

        if (session.SessionNumber.HasValue)
        {
            var lastAttendedSessionNumber = await dbContext.SessionAttendances
                .AsNoTracking()
                .Where(attendance => attendance.UserId == userId && attendance.WasPresent)
                .Where(attendance =>
                    attendance.Session.DeletedAt == null &&
                    attendance.Session.SubstanceCategoryId == session.SubstanceCategoryId &&
                    attendance.Session.SessionNumber.HasValue)
                .Select(attendance => (int?)attendance.Session.SessionNumber)
                .MaxAsync() ?? 0;

            var lastPaidSessionNumber = await dbContext.Payments
                .AsNoTracking()
                .Where(payment => payment.UserId == userId && payment.Status == "paid" && payment.Session != null)
                .Where(payment =>
                    payment.Session!.DeletedAt == null &&
                    payment.Session!.SubstanceCategoryId == session.SubstanceCategoryId &&
                    payment.Session!.SessionNumber.HasValue)
                .Select(payment => (int?)payment.Session!.SessionNumber)
                .MaxAsync() ?? 0;

            var lastSessionNumber = Math.Max(lastAttendedSessionNumber, lastPaidSessionNumber);
            var nextSessionNumber = lastSessionNumber + 1;

            if (session.SessionNumber.Value > nextSessionNumber)
            {
                return new SessionBookingResult(
                    false,
                    $"You must book session #{nextSessionNumber} first.",
                    StatusCodes.Status422UnprocessableEntity);
            }
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

    /// <summary>
    /// أي سيشن status بتاعها "live" بس فعليًا عدت المدة بتاعتها (StartedAt + DurationMinutes)
    /// بيتم تحويلها أوتوماتيك لـ "finished" قبل أي قراية للسيشنز.
    /// لازم يتنادي في أول أي method بترجع/تتأكد من سيشنز بحالة live.
    /// </summary>
    private async Task ExpireLiveSessionsAsync()
    {
        var now = DateTime.UtcNow;

        var liveSessions = await dbContext.TherapySessions
            .Where(session => session.Status == "live" && session.DeletedAt == null)
            .ToListAsync();

        var expiredSessions = liveSessions
            .Where(session => session.StartedAt.HasValue &&
                               session.StartedAt.Value.AddMinutes(session.DurationMinutes) <= now)
            .ToList();

        if (expiredSessions.Count == 0)
        {
            return;
        }

        foreach (var session in expiredSessions)
        {
            session.Status = "finished";
            session.EndedAt = session.StartedAt!.Value.AddMinutes(session.DurationMinutes);
            session.UpdatedAt = now;
        }

        await dbContext.SaveChangesAsync();
    }

    private IQueryable<TherapySession> ActiveSessionsQuery()
    {
        return dbContext.TherapySessions
            .AsNoTracking()
            .Include(session => session.SubstanceCategory)
            .Include(session => session.Instructor)
            .Include(session => session.Attendances)
            .Include(session => session.Payments)
            .Where(session => session.DeletedAt == null)
            /*.Where(session => session.Status != "finished")*/;
    }

    private static bool IsFull(TherapySession session)
    {
        return session.Attendances.Count(attendance => attendance.WasPresent) >= MaxParticipants(session);
    }

    private static int MaxParticipants(TherapySession session)
    {
        if (string.Equals(session.SessionType, IndividualSessionType, StringComparison.OrdinalIgnoreCase))
        {
            return 1;
        }

        var metadata = ReadMetadata(session.SessionMetadata);

        if (metadata.TryGetValue("max_participants", out var value) &&
            int.TryParse(Convert.ToString(value), out var maxParticipants) &&
            maxParticipants > 0)
        {
            return Math.Min(maxParticipants, MaxSessionParticipants);
        }

        return MaxSessionParticipants;
    }

    private async Task<bool> UserHasSubstanceCategoryAsync(long userId, long substanceCategoryId)
    {
        return await dbContext.UserSubstances
            .AsNoTracking()
            .AnyAsync(userSubstance =>
                userSubstance.UserId == userId &&
                userSubstance.Substance.SubstanceCategoryId == substanceCategoryId);
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
