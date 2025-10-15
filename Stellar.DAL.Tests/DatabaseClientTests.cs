using Microsoft.Data.SqlClient;

namespace Stellar.DAL.Tests;

public class DatabaseClientTests
{
    public const string ConnectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;Integrated Security=True;Initial Catalog=master";

    [Fact]
    public void Connects()
    {
        var client = new DatabaseClient<SqlConnection>(ConnectionString);

        var cmd = client
            .GetCommand()
            .SetCommandText("SELECT 1;");

        Assert.Equal(1, cmd.ExecuteScalar());
    }

    [Fact]
    public void GetsCommand()
    {
        var client = new DatabaseClient<SqlConnection>(ConnectionString);

        Assert.NotNull(client.GetCommand());
        Assert.NotNull(client.GetCommand(client.CreateConnection()));
        Assert.NotNull(client.GetCommand(client.CreateConnection().CreateCommand()));
        Assert.NotNull(client.GetCommand(new SqlCommand()));
    }

}
