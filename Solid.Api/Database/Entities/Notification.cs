namespace Solid.Api.Database.Entities;

public sealed class Notification
{
    public Guid Id { get; set; }

    public string Type { get; set; } = string.Empty;

    public string NotifiableType { get; set; } = string.Empty;

    public long NotifiableId { get; set; }

    public string Data { get; set; } = string.Empty;

    public DateTime? ReadAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }   // جديد

}
