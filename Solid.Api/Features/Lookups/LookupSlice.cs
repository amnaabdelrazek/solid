using Solid.Api.Common;
using Solid.Api.Database;

namespace Solid.Api.Features.Lookups;

public static class LookupSlice
{
    public static IEndpointRouteBuilder MapLookupSlice(this IEndpointRouteBuilder api)
    {
        api.MapGet("/lookups/substances", Substances);
        api.MapGet("/lookups/{type}", ByType);

        return api;
    }

    private static async Task<IResult> Substances(HttpRequest request, IDatabase database)
    {
        var locale = RequestCulture.Locale(request);
        var categories = (await database.QueryAsync(
            """
            SELECT id, name_ar, name_en
            FROM substance_categories
            WHERE is_active = 1
            ORDER BY sort_order, id
            """)).Select(row => new Dictionary<string, object?>(row, StringComparer.OrdinalIgnoreCase)).ToList();

        foreach (var category in categories)
        {
            var substances = await database.QueryAsync(
                """
                SELECT id, name_ar, name_en
                FROM substances
                WHERE substance_category_id = @categoryId AND is_active = 1
                ORDER BY id
                """,
                new { categoryId = category["id"] });

            category["label"] = category.Translated("name", locale);
            category["substances"] = substances.Select(substance => new
            {
                id = substance["id"],
                label = substance.Translated("name", locale)
            });
            category.Remove("name_ar");
            category.Remove("name_en");
        }

        return ApiResponse.Ok(new { categories });
    }

    private static async Task<IResult> ByType(string type, HttpRequest request, IDatabase database)
    {
        var locale = RequestCulture.Locale(request);
        var lookupType = await database.QuerySingleAsync("SELECT TOP 1 id FROM lookup_types WHERE [key] = @type", new { type });
        if (lookupType is null)
        {
            return ApiResponse.Fail("Not found.", StatusCodes.Status404NotFound);
        }

        var values = await database.QueryAsync(
            """
            SELECT id, value_key, label_ar, label_en
            FROM lookup_values
            WHERE lookup_type_id = @lookupTypeId AND is_active = 1
            ORDER BY sort_order, id
            """,
            new { lookupTypeId = lookupType["id"] });

        return ApiResponse.Ok(new
        {
            values = values.Select(value => new
            {
                id = value["id"],
                value_key = value["value_key"],
                label = value.Translated("label", locale)
            })
        });
    }
}
