namespace Solid.Api.Database.Entities;

public sealed class UserTreatmentType
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long LookupValueId { get; set; }

    public User User { get; set; } = null!;

    public LookupValue LookupValue { get; set; } = null!;
}
