namespace Solid.Api.Database.Entities;

public sealed class DeviceSession
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public string DeviceId { get; set; } = string.Empty;

    public string? DeviceInfo { get; set; }

    public string EventType { get; set; } = string.Empty;

    public long? SanctumTokenId { get; set; }

    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
}
