namespace Solid.Api.Database.Entities;

public sealed class Setting
{
    public long Id { get; set; }

    public string Group { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public bool Locked { get; set; }

    public string Payload { get; set; } = string.Empty;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
