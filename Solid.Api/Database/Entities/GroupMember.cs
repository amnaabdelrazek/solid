namespace Solid.Api.Database.Entities;

public sealed class GroupMember
{
    public long Id { get; set; }

    public long GroupId { get; set; }

    public long UserId { get; set; }

    public DateTime JoinedAt { get; set; }

    public bool IsActive { get; set; } = true;

    public Group Group { get; set; } = null!;

    public User User { get; set; } = null!;
}
