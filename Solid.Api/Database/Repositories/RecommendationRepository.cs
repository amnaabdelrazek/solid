namespace Solid.Api.Database.Repositories;

public sealed class RecommendationRepository(IDatabase database) : IRecommendationRepository
{
    public async Task<IReadOnlyList<Dictionary<string, object?>>> ActiveAsync()
    {
        return await database.QueryAsync("SELECT * FROM recommendations WHERE is_active = 1 ORDER BY id");
    }
}
