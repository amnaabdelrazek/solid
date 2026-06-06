namespace Solid.Api.Database.Entities;

public sealed class Recommendation
{
    public long Id { get; set; }

    public long? CreatedByUserId { get; set; }

    public long? SubstanceCategoryId { get; set; }

    public string Type { get; set; } = string.Empty;

    public string NameAr { get; set; } = string.Empty;

    public string NameEn { get; set; } = string.Empty;

    public string? ContactInfo { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public SubstanceCategory? SubstanceCategory { get; set; }

    public User? CreatedByUser { get; set; }
}
