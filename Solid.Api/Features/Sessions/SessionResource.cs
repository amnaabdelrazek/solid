using Solid.Api.Common;

namespace Solid.Api.Features.Sessions;

public static class SessionResource
{
    public static object From(Dictionary<string, object?> session)
    {
        var scheduledAt = session.Value<DateTime?>("scheduled_at");
        var sessionNumber = session.GetValueOrDefault("session_number");
        var status = Convert.ToString(session.GetValueOrDefault("status"));

        return new
        {
            id = session["id"],
            group_id = session.GetValueOrDefault("group_id"),
            instructor_id = session.GetValueOrDefault("instructor_id"),
            session_number = sessionNumber,
            title = $"Session {sessionNumber}",
            session_type = session.GetValueOrDefault("session_type"),
            session_type_label = Convert.ToString(session.GetValueOrDefault("session_type")) == "paid" ? "Paid Session" : "Group Session",
            status,
            scheduled_at = scheduledAt,
            date = scheduledAt?.ToString("ddd, MMM dd"),
            time = scheduledAt?.ToString("hh:mm tt"),
            started_at = session.GetValueOrDefault("started_at"),
            ended_at = session.GetValueOrDefault("ended_at"),
            duration_minutes = session.GetValueOrDefault("duration_minutes"),
            jitsi_room_name = session.GetValueOrDefault("jitsi_room_name"),
            jitsi_jwt_issued_at = session.GetValueOrDefault("jitsi_jwt_issued_at"),
            session_metadata = session.JsonValue("session_metadata"),
            price = 0,
            formatted_price = "0 EGP",
            created_at = session.GetValueOrDefault("created_at"),
            updated_at = session.GetValueOrDefault("updated_at"),
            is_booked = false,
            is_locked = status != "live"
        };
    }
}
