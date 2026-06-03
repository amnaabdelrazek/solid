namespace Solid.Api.Database.Entities;

public sealed class LookupType
{
    public long Id { get; set; }

    public string Key { get; set; } = string.Empty;

    public string LabelAr { get; set; } = string.Empty;

    public string LabelEn { get; set; } = string.Empty;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public ICollection<LookupValue> Values { get; set; } = [];
}
