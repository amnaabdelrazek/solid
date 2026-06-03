namespace Solid.Api.Database.Entities;

public sealed class TherapySession
{
    public long Id { get; set; }

    public long GroupId { get; set; }

    public long InstructorId { get; set; }

    public int? SessionNumber { get; set; }

    public string SessionType { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime ScheduledAt { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? EndedAt { get; set; }

    public byte DurationMinutes { get; set; } = 45;

    public string JitsiRoomName { get; set; } = string.Empty;

    public DateTime? JitsiJwtIssuedAt { get; set; }

    public string? SessionMetadata { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Group Group { get; set; } = null!;

    public User Instructor { get; set; } = null!;

    public ICollection<SessionAttendance> Attendances { get; set; } = [];

    public ICollection<Payment> Payments { get; set; } = [];
}
