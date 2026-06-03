namespace Solid.Api.Database.Entities;

public sealed class CacheLock
{
    public string Key { get; set; } = string.Empty;

    public string Owner { get; set; } = string.Empty;

    public long Expiration { get; set; }
}
