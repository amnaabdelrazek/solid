using Microsoft.EntityFrameworkCore;
using Solid.Api.Database.Entities;

namespace Solid.Api.Database.Repositories;

public sealed class RecommendationRepository(SolidDbContext dbContext) : IRecommendationRepository
{
    public async Task<IReadOnlyList<Recommendation>> ActiveAsync()
    {
        return await dbContext.Recommendations
            .AsNoTracking()
            .Where(recommendation => recommendation.IsActive)
            .OrderBy(recommendation => recommendation.Id)
            .ToListAsync();
    }

    public async Task<Recommendation?> FindUserRecommendationAsync(long id, long userId)
    {
        return await dbContext.Recommendations
            .FirstOrDefaultAsync(r => r.Id == id && r.CreatedByUserId == userId);
    }

    public async Task<Recommendation> CreateAsync(long userId, RecommendationCreate create)
    {
        var now = DateTime.UtcNow;
        var recommendation = new Recommendation
        {
            CreatedByUserId = userId,
            SubstanceCategoryId = create.SubstanceCategoryId,
            Type = create.Type,
            NameAr = create.NameAr,
            NameEn = create.NameEn ?? create.NameAr,
            ContactInfo = create.ContactInfo,
            Latitude = create.Latitude,
            Longitude = create.Longitude,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        dbContext.Recommendations.Add(recommendation);
        await dbContext.SaveChangesAsync();
        return recommendation;
    }

    public async Task<Recommendation?> UpdateAsync(long id, long userId, RecommendationUpdate update)
    {
        var recommendation = await dbContext.Recommendations
            .FirstOrDefaultAsync(r => r.Id == id && r.CreatedByUserId == userId);

        if (recommendation is null) return null;

        if (update.Type != null) recommendation.Type = update.Type;
        if (update.NameAr != null) recommendation.NameAr = update.NameAr;
        if (update.NameEn != null) recommendation.NameEn = update.NameEn;
        if (update.ContactInfo != null) recommendation.ContactInfo = update.ContactInfo;
        if (update.Latitude.HasValue) recommendation.Latitude = update.Latitude;
        if (update.Longitude.HasValue) recommendation.Longitude = update.Longitude;
        if (update.IsActive.HasValue) recommendation.IsActive = update.IsActive.Value;
        recommendation.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
        return recommendation;
    }

    public async Task DeactivateAsync(long id, long userId)
    {
        var recommendation = await dbContext.Recommendations
            .FirstOrDefaultAsync(r => r.Id == id && r.CreatedByUserId == userId);

        if (recommendation is null) return;

        recommendation.IsActive = false;
        recommendation.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();
    }
}
