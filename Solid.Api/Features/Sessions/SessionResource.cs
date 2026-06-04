using Solid.Api.Common;
using Solid.Api.Database.Entities;

namespace Solid.Api.Features.Sessions;

public static class SessionResource
{
    public static object From(TherapySession session)
    {
        return new
        {
            id = session.Id,
            group_id = session.GroupId,
            instructor_id = session.InstructorId,
            session_number = session.SessionNumber,
            title = $"Session {session.SessionNumber}",
            session_type = session.SessionType,
            session_type_label = session.SessionType == "paid" ? "Paid Session" : "Group Session",
            status = session.Status,
            scheduled_at = session.ScheduledAt,
            date = session.ScheduledAt.ToString("ddd, MMM dd"),
            time = session.ScheduledAt.ToString("hh:mm tt"),
            started_at = session.StartedAt,
            ended_at = session.EndedAt,
            duration_minutes = session.DurationMinutes,
            jitsi_room_name = session.JitsiRoomName,
            jitsi_jwt_issued_at = session.JitsiJwtIssuedAt,
            session_metadata = JsonPayload.Parse(session.SessionMetadata),
            price = 0,
            formatted_price = "0 EGP",
            created_at = session.CreatedAt,
            updated_at = session.UpdatedAt,
            is_booked = false,
            is_locked = session.Status != "live"
        };
    }
}
