using Solid.Api.Database.Entities;

namespace Solid.Api.Database.Repositories;

public interface ILookupRepository
{
    Task<IReadOnlyList<SubstanceCategory>> SubstanceCategoriesAsync();

    Task<IReadOnlyList<Substance>> SubstancesAsync(long categoryId);

    Task<LookupType?> LookupTypeAsync(string type);

    Task<IReadOnlyList<LookupValue>> LookupValuesAsync(long lookupTypeId);

    Task<int> CountLookupValuesAsync(IEnumerable<long> lookupValueIds);

    Task<int> CountSubstancesAsync(IEnumerable<long> substanceIds);
}
