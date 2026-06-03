namespace Solid.Api.Database.Entities;

public sealed class SessionAttendance
{
    public long Id { get; set; }

    public long SessionId { get; set; }

    public long UserId { get; set; }

    public DateTime? JoinedAt { get; set; }

    public DateTime? LeftAt { get; set; }

    public bool WasPresent { get; set; }

    public byte? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public TherapySession Session { get; set; } = null!;

    public User User { get; set; } = null!;
}
