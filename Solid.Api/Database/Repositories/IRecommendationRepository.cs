using Solid.Api.Database.Entities;

namespace Solid.Api.Database.Repositories;

public interface IRecommendationRepository
{
    Task<IReadOnlyList<Recommendation>> ActiveAsync();
}
