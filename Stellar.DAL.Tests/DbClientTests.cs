using Microsoft.Data.SqlClient;

namespace Stellar.DAL.Tests;

public class DbClientTests
{
    [Fact]
    public void Connects()
    {
        var connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;Integrated Security=True;Initial Catalog=master";

        var client = new DatabaseClient<SqlConnection>(connectionString);

        var cmd = client
            .GetCommand()
            .SetCommandText("SELECT 1;");

        Assert.Equal(1, cmd.ExecuteScalar());
    }
}
