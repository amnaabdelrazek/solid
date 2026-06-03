namespace Solid.Api.Database.Repositories;

public sealed class LookupRepository(IDatabase database) : ILookupRepository
{
    public async Task<IReadOnlyList<Dictionary<string, object?>>> SubstanceCategoriesAsync()
    {
        return await database.QueryAsync(
            """
            SELECT id, name_ar, name_en
            FROM substance_categories
            WHERE is_active = 1
            ORDER BY sort_order, id
            """);
    }

    public async Task<IReadOnlyList<Dictionary<string, object?>>> SubstancesAsync(object categoryId)
    {
        return await database.QueryAsync(
            """
            SELECT id, name_ar, name_en
            FROM substances
            WHERE substance_category_id = @categoryId AND is_active = 1
            ORDER BY id
            """,
            new { categoryId });
    }

    public async Task<Dictionary<string, object?>?> LookupTypeAsync(string type)
    {
        return await database.QuerySingleAsync("SELECT TOP 1 id FROM lookup_types WHERE [key] = @type", new { type });
    }

    public async Task<IReadOnlyList<Dictionary<string, object?>>> LookupValuesAsync(object lookupTypeId)
    {
        return await database.QueryAsync(
            """
            SELECT id, value_key, label_ar, label_en
            FROM lookup_values
            WHERE lookup_type_id = @lookupTypeId AND is_active = 1
            ORDER BY sort_order, id
            """,
            new { lookupTypeId });
    }
}
