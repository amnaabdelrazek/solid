namespace Solid.Api.Database.Entities;

public sealed class AddictionProfile
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long AddictionDurationId { get; set; }

    public long EducationLevelId { get; set; }

    public bool HadPriorTreatment { get; set; }

    public string? AddictionReason { get; set; }

    public int? DaysClean { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public User User { get; set; } = null!;

    public LookupValue AddictionDuration { get; set; } = null!;

    public LookupValue EducationLevel { get; set; } = null!;
}
