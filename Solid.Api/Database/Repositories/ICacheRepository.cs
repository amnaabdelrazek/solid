namespace Solid.Api.Database.Repositories;

public interface ICacheRepository
{
    Task PutAsync(string key, string value, int seconds);

    Task<string?> GetAsync(string key);

    Task ForgetAsync(string key);
}
