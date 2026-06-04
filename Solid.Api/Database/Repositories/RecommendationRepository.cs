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
}
