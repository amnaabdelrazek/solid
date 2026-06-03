namespace Solid.Api.Database.Entities;

public sealed class SubstanceCategory
{
    public long Id { get; set; }

    public string NameAr { get; set; } = string.Empty;

    public string NameEn { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public byte SortOrder { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public ICollection<Substance> Substances { get; set; } = [];

    public ICollection<Group> Groups { get; set; } = [];

    public ICollection<Recommendation> Recommendations { get; set; } = [];
}
