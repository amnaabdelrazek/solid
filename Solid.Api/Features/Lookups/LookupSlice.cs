using Solid.Api.Common;
using Solid.Api.Database.Entities;
using Solid.Api.Database.Repositories;

namespace Solid.Api.Features.Lookups;

public static class LookupSlice
{
    public static IEndpointRouteBuilder MapLookupSlice(this IEndpointRouteBuilder api)
    {
        api.MapGet("/lookups/substances", Substances);
        api.MapGet("/lookups/{type}", ByType);

        return api;
    }

    private static async Task<IResult> Substances(HttpRequest request, ILookupRepository lookupRepository)
    {
        var locale = RequestCulture.Locale(request);
        var categories = await lookupRepository.SubstanceCategoriesAsync();

        return ApiResponse.Ok(new
        {
            categories = await Task.WhenAll(categories.Select(async category =>
            {
                var substances = await lookupRepository.SubstancesAsync(category.Id);

                return new
                {
                    id = category.Id,
                    label = TranslatedName(category, locale),
                    substances = substances.Select(substance => new
                    {
                        id = substance.Id,
                        label = TranslatedName(substance, locale)
                    })
                };
            }))
        });
    }

    private static async Task<IResult> ByType(string type, HttpRequest request, ILookupRepository lookupRepository)
    {
        var locale = RequestCulture.Locale(request);
        var lookupType = await lookupRepository.LookupTypeAsync(type);
        if (lookupType is null)
        {
            return ApiResponse.Fail("Not found.", StatusCodes.Status404NotFound);
        }

        var values = await lookupRepository.LookupValuesAsync(lookupType.Id);

        return ApiResponse.Ok(new
        {
            values = values.Select(value => new
            {
                id = value.Id,
                value_key = value.ValueKey,
                label = TranslatedLabel(value, locale)
            })
        });
    }

    private static string TranslatedName(SubstanceCategory category, string locale)
    {
        return locale == "en" && !string.IsNullOrWhiteSpace(category.NameEn)
            ? category.NameEn
            : category.NameAr;
    }

    private static string TranslatedName(Substance substance, string locale)
    {
        return locale == "en" && !string.IsNullOrWhiteSpace(substance.NameEn)
            ? substance.NameEn
            : substance.NameAr;
    }

    private static string TranslatedLabel(LookupValue value, string locale)
    {
        return locale == "en" && !string.IsNullOrWhiteSpace(value.LabelEn)
            ? value.LabelEn
            : value.LabelAr;
    }
}
