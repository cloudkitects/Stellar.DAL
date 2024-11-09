namespace Stellar.DAL.Tests;

[Collection("Remote Database collection")]
public class RemoteDatabaseTests(RemoteDatabaseFixture fixture)
{
    readonly RemoteDatabaseFixture database = fixture;

    #region Execute
    /// <summary>
    /// Executes to dynamic list.
    /// </summary>
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

        dynamic customer = list[0];

        customer.FullName = $"{customer.Title} {customer.FirstName} {customer.MiddleName} {customer.LastName} {customer.Suffix}".Trim();

        Assert.Equal(fullName, customer.FullName);
    }
    #endregion

}
