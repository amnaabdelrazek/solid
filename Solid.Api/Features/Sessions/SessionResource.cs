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
            scheduled_at = session.ScheduledAt,
            date = session.ScheduledAt.ToString("ddd, MMM dd"),
            time = session.ScheduledAt.ToString("hh:mm tt"),
            started_at = session.StartedAt,
            ended_at = session.EndedAt,
            duration_minutes = session.DurationMinutes,
            jitsi_room_name = session.JitsiRoomName,
            jitsi_jwt_issued_at = session.JitsiJwtIssuedAt,
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
            created_at = session.CreatedAt,
            updated_at = session.UpdatedAt,
            is_booked = false,
            is_locked = session.Status != "live"
        };
    }
}
