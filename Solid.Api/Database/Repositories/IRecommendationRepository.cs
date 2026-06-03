namespace Solid.Api.Database.Repositories;

public interface IRecommendationRepository
{
    Task<IReadOnlyList<Dictionary<string, object?>>> ActiveAsync();
}
