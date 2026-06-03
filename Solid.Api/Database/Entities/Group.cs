namespace Solid.Api.Database.Entities;

public sealed class Group
{
    public long Id { get; set; }

    public long? InstructorId { get; set; }

    public long? SubstanceCategoryId { get; set; }

    public string GroupType { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string NameAr { get; set; } = string.Empty;

    public string NameEn { get; set; } = string.Empty;

    public byte MinMembers { get; set; } = 7;

    public byte MaxMembers { get; set; } = 15;

    public DateTime? DeletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public User? Instructor { get; set; }

    public SubstanceCategory? SubstanceCategory { get; set; }

    public ICollection<GroupMember> Members { get; set; } = [];

    public ICollection<TherapySession> Sessions { get; set; } = [];
}
