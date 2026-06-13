using Solid.Api.Common;
using Solid.Api.Database.Entities;

namespace Solid.Api.Features.Sessions;

public static class SessionResource
{
    public static object From(TherapySession session)
    {
        var metadata = JsonPayload.ParseObject(session.SessionMetadata);
        var title = Convert.ToString(metadata.GetValueOrDefault("title"));
        var maxParticipants = Convert.ToString(metadata.GetValueOrDefault("max_participants"));
        var currentParticipants = session.Attendances.Count(attendance => attendance.WasPresent);

        return new
        {
            id = session.Id,
            group_id = session.GroupId,
            instructor_id = session.InstructorId,
            session_number = session.SessionNumber,
            title = string.IsNullOrWhiteSpace(title) ? $"Session {session.SessionNumber}" : title,
            session_type = session.SessionType,
            session_type_label = session.SessionType == "paid" ? "Paid Session" : "Group Session",
            status = session.Status,
            scheduled_at = EgyptDateTime.Format(session.ScheduledAt),
            date = EgyptDateTime.Date(session.ScheduledAt),
            time = EgyptDateTime.Time(session.ScheduledAt),
            started_at = EgyptDateTime.Format(session.StartedAt),
            ended_at = EgyptDateTime.Format(session.EndedAt),
            duration_minutes = session.DurationMinutes,
            jitsi_room_name = session.JitsiRoomName,
            jitsi_jwt_issued_at = EgyptDateTime.Format(session.JitsiJwtIssuedAt),
            session_metadata = JsonPayload.Parse(session.SessionMetadata),
            max_participants = int.TryParse(maxParticipants, out var parsedMaxParticipants)
                ? parsedMaxParticipants
                : session.Group is null ? null : (int?)session.Group.MaxMembers,
            current_participants = currentParticipants,
            is_full = int.TryParse(maxParticipants, out var fullMaxParticipants)
                ? currentParticipants >= fullMaxParticipants
                : session.Group is not null && currentParticipants >= session.Group.MaxMembers,
            price = 0,
            formatted_price = "0 EGP",
            created_at = EgyptDateTime.Format(session.CreatedAt),
            updated_at = EgyptDateTime.Format(session.UpdatedAt),
            is_booked = false,
            is_locked = session.Status != "live"
        };
    }
}
