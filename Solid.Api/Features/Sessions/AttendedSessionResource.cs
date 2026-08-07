using Solid.Api.Common;
using Solid.Api.Database.Entities;

namespace Solid.Api.Features.Sessions;

public static class AttendedSessionResource
{
    public static object From(TherapySession session, long userId, decimal price = 0)
    {
        var attendance = session.Attendances.FirstOrDefault(a => a.UserId == userId);

        return new
        {
            id = session.Id,
            substance_category_id = session.SubstanceCategoryId,
            substance_category_name_ar = session.SubstanceCategory?.NameAr,
            substance_category_name_en = session.SubstanceCategory?.NameEn,
            instructor_id = session.InstructorId,
            instructor_name = session.Instructor?.DisplayName,
            session_number = session.SessionNumber,
            session_type = session.SessionType,
            session_type_label = string.Equals(session.SessionType, "individual", StringComparison.OrdinalIgnoreCase) ? "Individual Session" : "Group Session",
            status = session.Status,
            scheduled_at = EgyptDateTime.Format(session.ScheduledAt),
            date = EgyptDateTime.Date(session.ScheduledAt),
            time = EgyptDateTime.Time(session.ScheduledAt),
            started_at = EgyptDateTime.Format(session.StartedAt),
            ended_at = EgyptDateTime.Format(session.EndedAt),
            duration_minutes = session.DurationMinutes,
            price = price,
            formatted_price = $"{price:0.##} EGP",
            attendance = attendance is null ? null : new
            {
                joined_at = EgyptDateTime.Format(attendance.JoinedAt),
                left_at = EgyptDateTime.Format(attendance.LeftAt),
                was_present = attendance.WasPresent,
                has_rated = attendance.Rating.HasValue,
                rating = attendance.Rating,
                comment = attendance.Comment
            }
        };
    }
}
