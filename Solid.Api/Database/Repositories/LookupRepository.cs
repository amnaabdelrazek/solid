using Microsoft.EntityFrameworkCore;
using Solid.Api.Database.Entities;

namespace Solid.Api.Database.Repositories;

public sealed class LookupRepository(SolidDbContext dbContext) : ILookupRepository
{
    public async Task<IReadOnlyList<SubstanceCategory>> SubstanceCategoriesAsync()
    {
        return await dbContext.SubstanceCategories
            .AsNoTracking()
            .Where(category => category.IsActive)
            .OrderBy(category => category.SortOrder)
            .ThenBy(category => category.Id)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Substance>> SubstancesAsync(long categoryId)
    {
        return await dbContext.Substances
            .AsNoTracking()
            .Where(substance => substance.SubstanceCategoryId == categoryId && substance.IsActive)
            .OrderBy(substance => substance.Id)
            .ToListAsync();
    }

    public async Task<LookupType?> LookupTypeAsync(string type)
    {
        return await dbContext.LookupTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(lookupType => lookupType.Key == type);
    }

    public async Task<IReadOnlyList<LookupValue>> LookupValuesAsync(long lookupTypeId)
    {
        return await dbContext.LookupValues
            .AsNoTracking()
            .Where(value => value.LookupTypeId == lookupTypeId && value.IsActive)
            .OrderBy(value => value.SortOrder)
            .ThenBy(value => value.Id)
            .ToListAsync();
    }

    public async Task<int> CountLookupValuesAsync(IEnumerable<long> lookupValueIds)
    {
        var ids = lookupValueIds.Distinct().ToArray();

        return await dbContext.LookupValues.CountAsync(value => ids.Contains(value.Id));
    }

    public async Task<IReadOnlyList<Substance>> AllSubstancesAsync()
    {
        return await dbContext.Substances
            .AsNoTracking()
            .Where(x => x.IsActive)
            .ToListAsync();
    }
    public async Task<int> CountSubstancesAsync(IEnumerable<long> substanceIds)
    {
        var ids = substanceIds.Distinct().ToArray();

        return await dbContext.Substances.CountAsync(substance => ids.Contains(substance.Id));
    }
}
