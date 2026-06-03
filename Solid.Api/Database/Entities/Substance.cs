namespace Solid.Api.Database.Entities;

public sealed class Substance
{
    public long Id { get; set; }

    public long SubstanceCategoryId { get; set; }

    public string NameAr { get; set; } = string.Empty;

    public string NameEn { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public SubstanceCategory SubstanceCategory { get; set; } = null!;

    public ICollection<UserSubstance> UserSubstances { get; set; } = [];
}
