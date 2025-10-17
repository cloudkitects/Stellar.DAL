using Microsoft.Data.SqlClient;

namespace Stellar.DAL.Tests;

[Collection("Remote Database collection")]
public class RemoteDatabaseTests(RemoteDatabaseFixture fixture)
{
    readonly RemoteDatabaseFixture database = fixture;

    [Fact]
    public void Connects()
    {
        var client = new AzureDatabaseClient(database.ConnectionString);

        var command = client
            .GetCommand()
            .SetCommandText("SELECT 1;");

        Assert.Equal(1, command.ExecuteScalar());
    }

    [Fact]
    public void GetsCommand()
    {
        Assert.NotNull(database.GetCommand());
    }

    #region
    [Theory]
    [InlineData("SELECT TABLE_NAME\r\nFROM INFORMATION_SCHEMA.TABLES\r\nWHERE TABLE_SCHEMA = @schema\r\nORDER BY 1;", "SalesLT", 13)]
    public void ExecutesQueryToList(string sql, string schema, int count)
    {
        var list = database
            .GetCommand()
            .SetCommandText(sql)
            .AddParameter("schema", schema)
            .ExecuteToList<string>();

        Assert.Equal(count, list.Count);
    }

    [Theory]
    [InlineData(1, "Mr. Orlando N. Gee")]
    [InlineData(529, "Ms. Jeanie R. Glenn PhD")]
    public void ExecutesToDynamic(int customerId, string fullName)
    {
        var sql = "SELECT * FROM [SalesLT].[Customer] WHERE CustomerID = @customerId;";

        var customer = database.GetCommand()
            .SetCommandTimeout(60)
            .SetCommandText(sql)
            .AddParameter("@customerId", customerId, System.Data.DbType.Int32)
            .ExecuteToDynamic();

        customer!.FullName = $"{customer.Title} {customer.FirstName} {customer.MiddleName} {customer.LastName} {customer.Suffix}".Trim();

        Assert.Equal(fullName, customer.FullName);
    }

    [Theory]
    [InlineData(1, "Mr. Orlando N. Gee")]
    [InlineData(529, "Ms. Jeanie R. Glenn PhD")]
    public void ExecutesToDynamicList(int customerId, string fullName)
    {
        var sql = "SELECT * FROM [SalesLT].[Customer] WHERE CustomerID = @customerId;";

        var list = database.GetCommand()
            .SetCommandTimeout(60)
            .SetCommandText(sql)
            .AddParameter("@customerId", customerId, System.Data.DbType.Int32)
            .ExecuteToDynamicList();

        var customer = list.FirstOrDefault();

        customer!.FullName = $"{customer.Title} {customer.FirstName} {customer.MiddleName} {customer.LastName} {customer.Suffix}".Trim();

        Assert.Equal(fullName, customer.FullName);
    }
    #endregion
}
