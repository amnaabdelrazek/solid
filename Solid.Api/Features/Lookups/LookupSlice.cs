using Microsoft.AspNetCore.Mvc;
using Solid.Api.Common;
using Solid.Api.Database.Entities;
using Solid.Api.Database.Repositories;
using Solid.Api.Infrastructure.Auth;

namespace Solid.Api.Features.Lookups;

public static class LookupSlice
{
    public static IEndpointRouteBuilder MapLookupSlice(this IEndpointRouteBuilder api)
    {
        // Public
        api.MapGet("/lookups/substances", Substances);
        api.MapGet("/lookups/{type}", ByType);

        // Protected - require auth
        api.MapGet("/lookups", AllLookupTypes).RequireAuthorization();
        api.MapPost("/lookups", CreateLookupType).RequireAuthorization();
        api.MapPost("/lookups/{type}/values", AddLookupValue).RequireAuthorization();
        api.MapPut("/lookups/{type}/values/{valueId:long}", UpdateLookupValue).RequireAuthorization();
        api.MapDelete("/lookups/{type}/values/{valueId:long}", DeleteLookupValue).RequireAuthorization();

        return api;
    }

    // GET /api/lookups  — list all lookup types with their values
    private static async Task<IResult> AllLookupTypes(HttpRequest request, ILookupRepository lookupRepository)
    {
        var locale = RequestCulture.Locale(request);
        var types = await lookupRepository.AllLookupTypesAsync();

        var result = types.Select(t => new
        {
            id = t.Id,
            key = t.Key,
            label = locale == "en" ? t.LabelEn : t.LabelAr,
            label_ar = t.LabelAr,
            label_en = t.LabelEn,
            values = t.Values.Where(v => v.IsActive).OrderBy(v => v.SortOrder).ThenBy(v => v.Id).Select(v => new
            {
                id = v.Id,
                value_key = v.ValueKey,
                label = locale == "en" ? v.LabelEn : v.LabelAr,
                label_ar = v.LabelAr,
                label_en = v.LabelEn,
                sort_order = v.SortOrder,
                is_active = v.IsActive
            })
        });

        return ApiResponse.Ok(new { lookup_types = result });
    }

    // POST /api/lookups — create new lookup type
    private static async Task<IResult> CreateLookupType(
        IAuthContext auth,
        [FromBody] CreateLookupTypeRequest request,
        ILookupRepository lookupRepository)
    {
        if (string.IsNullOrWhiteSpace(request.key) || string.IsNullOrWhiteSpace(request.label_ar))
            return ApiResponse.Fail("Key and label_ar are required.", StatusCodes.Status422UnprocessableEntity);

        var key = request.key.Trim().ToLowerInvariant().Replace(" ", "_");
        var existing = await lookupRepository.LookupTypeAsync(key);
        if (existing is not null)
            return ApiResponse.Fail("A lookup type with this key already exists.", StatusCodes.Status422UnprocessableEntity);

        var created = await lookupRepository.CreateLookupTypeAsync(key, request.label_ar, request.label_en ?? request.label_ar);
        return ApiResponse.Ok(new
        {
            lookup_type = new
            {
                id = created.Id,
                key = created.Key,
                label_ar = created.LabelAr,
                label_en = created.LabelEn
            }
        }, "Lookup type created successfully.");
    }

    // POST /api/lookups/{type}/values — add value to lookup type
    private static async Task<IResult> AddLookupValue(
        string type,
        IAuthContext auth,
        [FromBody] CreateLookupValueRequest request,
        ILookupRepository lookupRepository)
    {
        var lookupType = await lookupRepository.LookupTypeAsync(type);
        if (lookupType is null)
            return ApiResponse.Fail("Lookup type not found.", StatusCodes.Status404NotFound);

        if (string.IsNullOrWhiteSpace(request.value_key) || string.IsNullOrWhiteSpace(request.label_ar))
            return ApiResponse.Fail("value_key and label_ar are required.", StatusCodes.Status422UnprocessableEntity);

        var valueKey = request.value_key.Trim().ToLowerInvariant().Replace(" ", "_");
        var created = await lookupRepository.CreateLookupValueAsync(lookupType.Id, valueKey, request.label_ar, request.label_en ?? request.label_ar, request.sort_order ?? 0);

        return ApiResponse.Ok(new
        {
            value = new
            {
                id = created.Id,
                lookup_type_id = created.LookupTypeId,
                value_key = created.ValueKey,
                label_ar = created.LabelAr,
                label_en = created.LabelEn,
                sort_order = created.SortOrder,
                is_active = created.IsActive
            }
        }, "Lookup value added successfully.");
    }

    // PUT /api/lookups/{type}/values/{valueId} — edit lookup value
    private static async Task<IResult> UpdateLookupValue(
        string type,
        long valueId,
        IAuthContext auth,
        [FromBody] UpdateLookupValueRequest request,
        ILookupRepository lookupRepository)
    {
        var lookupType = await lookupRepository.LookupTypeAsync(type);
        if (lookupType is null)
            return ApiResponse.Fail("Lookup type not found.", StatusCodes.Status404NotFound);

        var updated = await lookupRepository.UpdateLookupValueAsync(valueId, lookupType.Id, request.label_ar, request.label_en, request.sort_order, request.is_active);
        if (updated is null)
            return ApiResponse.Fail("Lookup value not found.", StatusCodes.Status404NotFound);

        return ApiResponse.Ok(new
        {
            value = new
            {
                id = updated.Id,
                value_key = updated.ValueKey,
                label_ar = updated.LabelAr,
                label_en = updated.LabelEn,
                sort_order = updated.SortOrder,
                is_active = updated.IsActive
            }
        }, "Lookup value updated successfully.");
    }

    // DELETE /api/lookups/{type}/values/{valueId}
    private static async Task<IResult> DeleteLookupValue(
        string type,
        long valueId,
        IAuthContext auth,
        ILookupRepository lookupRepository)
    {
        var lookupType = await lookupRepository.LookupTypeAsync(type);
        if (lookupType is null)
            return ApiResponse.Fail("Lookup type not found.", StatusCodes.Status404NotFound);

        await lookupRepository.DeactivateLookupValueAsync(valueId, lookupType.Id);
        return ApiResponse.Ok(message: "Lookup value removed.");
    }

    // GET /api/lookups/substances
    private static async Task<IResult> Substances(HttpRequest request, ILookupRepository lookupRepository)
    {
        var locale = RequestCulture.Locale(request);
        var categories = await lookupRepository.SubstanceCategoriesAsync();
        var allSubstances = await lookupRepository.AllSubstancesAsync();

        var result = categories.Select(category => new
        {
            id = category.Id,
            label = TranslatedName(category, locale),
            substances = allSubstances
                .Where(x => x.SubstanceCategoryId == category.Id)
                .Select(x => new
                {
                    id = x.Id,
                    label = TranslatedName(x, locale)
                })
        });

        return ApiResponse.Ok(new { categories = result });
    }

    // GET /api/lookups/{type}
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
        => locale == "en" && !string.IsNullOrWhiteSpace(category.NameEn) ? category.NameEn : category.NameAr;

    private static string TranslatedName(Substance substance, string locale)
        => locale == "en" && !string.IsNullOrWhiteSpace(substance.NameEn) ? substance.NameEn : substance.NameAr;

    private static string TranslatedLabel(LookupValue value, string locale)
        => locale == "en" && !string.IsNullOrWhiteSpace(value.LabelEn) ? value.LabelEn : value.LabelAr;
}

public sealed record CreateLookupTypeRequest(string key, string label_ar, string? label_en);
public sealed record CreateLookupValueRequest(string value_key, string label_ar, string? label_en, byte? sort_order);
public sealed record UpdateLookupValueRequest(string? label_ar, string? label_en, byte? sort_order, bool? is_active);
