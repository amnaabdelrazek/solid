using System.Data;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace Solid.Api.Database;

public sealed class SqlDatabase(ISqlConnectionFactory connectionFactory) : IDatabase
{
    public async Task<IReadOnlyList<Dictionary<string, object?>>> QueryAsync(string sql, object? parameters = null)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync();
        await using var command = BuildCommand(connection, sql, parameters);
        await using var reader = await command.ExecuteReaderAsync();

        var rows = new List<Dictionary<string, object?>>();
        while (await reader.ReadAsync())
        {
            rows.Add(ReadRow(reader));
        }

        return rows;
    }

    public async Task<Dictionary<string, object?>?> QuerySingleAsync(string sql, object? parameters = null)
    {
        return (await QueryAsync(sql, parameters)).FirstOrDefault();
    }

    public async Task<int> ExecuteAsync(string sql, object? parameters = null)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync();
        await using var command = BuildCommand(connection, sql, parameters);

        return await command.ExecuteNonQueryAsync();
    }

    public async Task<T> ExecuteScalarAsync<T>(string sql, object? parameters = null)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync();
        await using var command = BuildCommand(connection, sql, parameters);
        var value = await command.ExecuteScalarAsync();

        return (T)Convert.ChangeType(value!, typeof(T));
    }

    private static SqlCommand BuildCommand(SqlConnection connection, string sql, object? parameters)
    {
        var command = connection.CreateCommand();
        command.CommandText = sql;
        command.CommandType = CommandType.Text;

        if (parameters is null)
        {
            return command;
        }

        var parameterNames = Regex.Matches(sql, @"@[A-Za-z_][A-Za-z0-9_]*")
            .Select(match => match.Value[1..])
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var property in parameters.GetType().GetProperties())
        {
            if (!parameterNames.Contains(property.Name))
            {
                continue;
            }

            command.Parameters.AddWithValue($"@{property.Name}", property.GetValue(parameters) ?? DBNull.Value);
        }

        return command;
    }

    private static Dictionary<string, object?> ReadRow(SqlDataReader reader)
    {
        var row = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        for (var index = 0; index < reader.FieldCount; index++)
        {
            row[reader.GetName(index)] = reader.IsDBNull(index) ? null : reader.GetValue(index);
        }

        return row;
    }
}
