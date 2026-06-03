namespace Solid.Api.Database.Entities;

public sealed class LookupValue
{
    public long Id { get; set; }

    public long LookupTypeId { get; set; }

    public string ValueKey { get; set; } = string.Empty;

    public string LabelAr { get; set; } = string.Empty;

    public string LabelEn { get; set; } = string.Empty;

    public byte SortOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public LookupType LookupType { get; set; } = null!;

    public ICollection<AddictionProfile> AddictionDurationProfiles { get; set; } = [];

    public ICollection<AddictionProfile> EducationLevelProfiles { get; set; } = [];
}
