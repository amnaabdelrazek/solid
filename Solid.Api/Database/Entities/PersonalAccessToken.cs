namespace Solid.Api.Database.Entities;

public sealed class PersonalAccessToken
{
    public long Id { get; set; }

    public string TokenableType { get; set; } = string.Empty;

    public long TokenableId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;

    public string? Abilities { get; set; }

    public DateTime? LastUsedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
