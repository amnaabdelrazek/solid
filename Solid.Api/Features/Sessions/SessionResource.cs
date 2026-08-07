using Solid.Api.Common;
using Solid.Api.Database.Entities;

public static class SessionResource
{
    public static object From(TherapySession session, decimal price = 0, long? userId = null)
    {
        var metadata = JsonPayload.ParseObject(session.SessionMetadata);
        var title = Convert.ToString(metadata.GetValueOrDefault("title"));
        var maxParticipants = Convert.ToString(metadata.GetValueOrDefault("max_participants"));
        var userAttendance = userId.HasValue
            ? session.Attendances.FirstOrDefault(attendance => attendance.UserId == userId.Value)
            : null;
        var isBooked = userId.HasValue && session.Payments.Any(payment =>
            payment.UserId == userId.Value &&
            payment.Status == "paid");
        var attendanceStatus = userId.HasValue && (isBooked || userAttendance is not null)
            ? userAttendance?.WasPresent == true ? "attended" : "absent"
            : null;
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
            session_type_label = string.Equals(session.SessionType, "individual", StringComparison.OrdinalIgnoreCase) ? "Individual Session" : "Group Session",
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
                ? MaxParticipants(session, parsedMaxParticipants)
                : MaxParticipants(session, null),
            current_participants = currentParticipants,
            is_full = currentParticipants >= (int.TryParse(maxParticipants, out var fullMaxParticipants)
                ? MaxParticipants(session, fullMaxParticipants)
                : MaxParticipants(session, null)),
            price = price,                              // USE PARAM
            formatted_price = $"{price:0.##} EGP",     // USE PARAM
            created_at = EgyptDateTime.Format(session.CreatedAt),
            updated_at = EgyptDateTime.Format(session.UpdatedAt),
            is_booked = isBooked,
            attendance_status = attendanceStatus,
            is_locked = session.Status != "live"
        };
    }

    private static int MaxParticipants(TherapySession session, int? metadataMaxParticipants)
    {
        if (string.Equals(session.SessionType, "individual", StringComparison.OrdinalIgnoreCase))
        {
            return 1;
        }

        return metadataMaxParticipants is > 0
            ? Math.Min(metadataMaxParticipants.Value, 15)
            : 15;
    }
}
