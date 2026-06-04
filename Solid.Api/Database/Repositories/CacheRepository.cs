using Microsoft.EntityFrameworkCore;
using Solid.Api.Database.Entities;

namespace Solid.Api.Database.Repositories;

public sealed class CacheRepository(SolidDbContext dbContext) : ICacheRepository
{
    public async Task PutAsync(string key, string value, int seconds)
    {
        var expiration = DateTimeOffset.UtcNow.AddSeconds(seconds).ToUnixTimeSeconds();
        var cacheEntry = await dbContext.Cache.FirstOrDefaultAsync(entry => entry.Key == key);

        if (cacheEntry is null)
        {
            dbContext.Cache.Add(new CacheEntry
            {
                Key = key,
                Value = value,
                Expiration = expiration
            });
        }
        else
        {
            cacheEntry.Value = value;
            cacheEntry.Expiration = expiration;
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task<string?> GetAsync(string key)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        return await dbContext.Cache
            .AsNoTracking()
            .Where(entry => entry.Key == key && entry.Expiration > now)
            .Select(entry => entry.Value)
            .FirstOrDefaultAsync();
    }

    public async Task ForgetAsync(string key)
    {
        await dbContext.Cache
            .Where(entry => entry.Key == key)
            .ExecuteDeleteAsync();
    }
}
