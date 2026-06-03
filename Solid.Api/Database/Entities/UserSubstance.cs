namespace Solid.Api.Database.Entities;

public sealed class UserSubstance
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long SubstanceId { get; set; }

    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;

    public Substance Substance { get; set; } = null!;
}
