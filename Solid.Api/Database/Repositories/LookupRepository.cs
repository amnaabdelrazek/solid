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

    public async Task<IReadOnlyList<LookupType>> AllLookupTypesAsync()
    {
        return await dbContext.LookupTypes
            .AsNoTracking()
            .Include(t => t.Values)
            .OrderBy(t => t.Id)
            .ToListAsync();
    }

    public async Task<LookupType> CreateLookupTypeAsync(string key, string labelAr, string labelEn)
    {
        var now = DateTime.UtcNow;
        var lookupType = new LookupType
        {
            Key = key,
            LabelAr = labelAr,
            LabelEn = labelEn,
            CreatedAt = now,
            UpdatedAt = now
        };
        dbContext.LookupTypes.Add(lookupType);
        await dbContext.SaveChangesAsync();
        return lookupType;
    }

    public async Task<LookupValue> CreateLookupValueAsync(long lookupTypeId, string valueKey, string labelAr, string labelEn, byte sortOrder)
    {
        var now = DateTime.UtcNow;
        var value = new LookupValue
        {
            LookupTypeId = lookupTypeId,
            ValueKey = valueKey,
            LabelAr = labelAr,
            LabelEn = labelEn,
            SortOrder = sortOrder,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };
        dbContext.LookupValues.Add(value);
        await dbContext.SaveChangesAsync();
        return value;
    }

    public async Task<LookupValue?> UpdateLookupValueAsync(long valueId, long lookupTypeId, string? labelAr, string? labelEn, byte? sortOrder, bool? isActive)
    {
        var value = await dbContext.LookupValues.FirstOrDefaultAsync(v => v.Id == valueId && v.LookupTypeId == lookupTypeId);
        if (value is null) return null;

        if (labelAr != null) value.LabelAr = labelAr;
        if (labelEn != null) value.LabelEn = labelEn;
        if (sortOrder.HasValue) value.SortOrder = sortOrder.Value;
        if (isActive.HasValue) value.IsActive = isActive.Value;
        value.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
        return value;
    }

    public async Task DeactivateLookupValueAsync(long valueId, long lookupTypeId)
    {
        var value = await dbContext.LookupValues.FirstOrDefaultAsync(v => v.Id == valueId && v.LookupTypeId == lookupTypeId);
        if (value is null) return;

        value.IsActive = false;
        value.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();
    }
}
