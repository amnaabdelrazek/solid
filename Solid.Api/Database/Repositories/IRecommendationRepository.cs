using Solid.Api.Database.Entities;

namespace Solid.Api.Database.Repositories;

public interface IRecommendationRepository
{
    Task<IReadOnlyList<Recommendation>> ActiveAsync();

    Task<Recommendation?> FindUserRecommendationAsync(long id, long userId);

    Task<Recommendation> CreateAsync(long userId, RecommendationCreate create);

    Task<Recommendation?> UpdateAsync(long id, long userId, RecommendationUpdate update);

    Task DeactivateAsync(long id, long userId);
}

public sealed record RecommendationCreate(
    long? SubstanceCategoryId,
    string Type,
    string NameAr,
    string? NameEn,
    string? ContactInfo,
    decimal? Latitude,
    decimal? Longitude);

public sealed record RecommendationUpdate(
    string? Type,
    string? NameAr,
    string? NameEn,
    string? ContactInfo,
    decimal? Latitude,
    decimal? Longitude,
    bool? IsActive);
