using Microsoft.Data.SqlClient;

namespace Solid.Api.Database;

public interface ISqlConnectionFactory
{
    SqlConnection Create();
}
