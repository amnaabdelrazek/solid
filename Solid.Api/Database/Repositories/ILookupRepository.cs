using Solid.Api.Database.Entities;

namespace Solid.Api.Database.Repositories;

public interface ILookupRepository
{
    Task<IReadOnlyList<SubstanceCategory>> SubstanceCategoriesAsync();

    Task<IReadOnlyList<Substance>> SubstancesAsync(long categoryId);

    Task<LookupType?> LookupTypeAsync(string type);

    Task<IReadOnlyList<Substance>> AllSubstancesAsync();

    Task<IReadOnlyList<LookupValue>> LookupValuesAsync(long lookupTypeId);

    Task<int> CountLookupValuesAsync(IEnumerable<long> lookupValueIds);

    Task<int> CountSubstancesAsync(IEnumerable<long> substanceIds);

    // New methods
    Task<IReadOnlyList<LookupType>> AllLookupTypesAsync();

    Task<LookupType> CreateLookupTypeAsync(string key, string labelAr, string labelEn);

    Task<LookupValue> CreateLookupValueAsync(long lookupTypeId, string valueKey, string labelAr, string labelEn, byte sortOrder);

    Task<LookupValue?> UpdateLookupValueAsync(long valueId, long lookupTypeId, string? labelAr, string? labelEn, byte? sortOrder, bool? isActive);

    Task DeactivateLookupValueAsync(long valueId, long lookupTypeId);
}
