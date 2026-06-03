namespace Solid.Api.Database.Repositories;

public interface ILookupRepository
{
    Task<IReadOnlyList<Dictionary<string, object?>>> SubstanceCategoriesAsync();

    Task<IReadOnlyList<Dictionary<string, object?>>> SubstancesAsync(object categoryId);

    Task<Dictionary<string, object?>?> LookupTypeAsync(string type);

    Task<IReadOnlyList<Dictionary<string, object?>>> LookupValuesAsync(object lookupTypeId);
}
