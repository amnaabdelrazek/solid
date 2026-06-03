namespace Solid.Api.Database.Entities;

public sealed class CacheEntry
{
    public string Key { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public long Expiration { get; set; }
}
