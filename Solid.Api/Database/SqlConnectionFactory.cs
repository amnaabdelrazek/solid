using Microsoft.Data.SqlClient;

namespace Solid.Api.Database;

public sealed class SqlConnectionFactory(IConfiguration configuration) : ISqlConnectionFactory
{
    public SqlConnection Create()
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required.");

        return new SqlConnection(connectionString);
    }
}
