namespace Solid.Api.Database;

public interface IDatabase
{
    Task<IReadOnlyList<Dictionary<string, object?>>> QueryAsync(string sql, object? parameters = null);

    Task<Dictionary<string, object?>?> QuerySingleAsync(string sql, object? parameters = null);

    Task<int> ExecuteAsync(string sql, object? parameters = null);

    Task<T> ExecuteScalarAsync<T>(string sql, object? parameters = null);
}
