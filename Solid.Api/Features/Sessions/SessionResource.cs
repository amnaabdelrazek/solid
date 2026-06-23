using Solid.Api.Common;
using Solid.Api.Database.Entities;

public static class SessionResource
{
    public static object From(TherapySession session, decimal price = 0)
    {
        var metadata = JsonPayload.ParseObject(session.SessionMetadata);
        var title = Convert.ToString(metadata.GetValueOrDefault("title"));
        var maxParticipants = Convert.ToString(metadata.GetValueOrDefault("max_participants"));
        var paidParticipants = session.Payments
            .Where(payment => payment.Status == "paid")
            .Select(payment => payment.UserId)
            .Distinct()
            .Count();
        var attendedParticipants = session.Attendances.Count(attendance => attendance.WasPresent);
        var currentParticipants = Math.Max(paidParticipants, attendedParticipants);

        return new
        {
            id = session.Id,
            group_id = session.SubstanceCategoryId,
            group_name = session.SubstanceCategory?.NameAr,
            group_name_ar = session.SubstanceCategory?.NameAr,
            group_name_en = session.SubstanceCategory?.NameEn,
            instructor_id = session.InstructorId,
            instructor_name = session.Instructor?.DisplayName,
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
                ? Math.Min(parsedMaxParticipants, 15)
                : 15,
            current_participants = currentParticipants,
            is_full = int.TryParse(maxParticipants, out var fullMaxParticipants)
                ? currentParticipants >= Math.Min(fullMaxParticipants, 15)
                : currentParticipants >= 15,
            price = price,                              // USE PARAM
            formatted_price = $"{price:0.##} EGP",     // USE PARAM
            created_at = EgyptDateTime.Format(session.CreatedAt),
            updated_at = EgyptDateTime.Format(session.UpdatedAt),
            is_booked = false,
            is_locked = session.Status != "live"
        };
    }
}
