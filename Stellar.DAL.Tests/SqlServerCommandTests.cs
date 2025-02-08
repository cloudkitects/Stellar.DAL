using System.Transactions;

using Microsoft.Data.SqlClient;

using Stellar.DAL.Tests.Data;

namespace Stellar.DAL.Tests;

[Collection("Local Database collection")]
public class SqlServerCommandTests(LocalSqlDatabaseFixture fixture)
{
    readonly LocalSqlDatabaseFixture database = fixture;

    #region debug command
    [Fact]
    public void GetsDebugCommandText()
    {
        var customers = Seed.Customers.ToList();

        var command = database.GetCommand().GenerateSqlServerInserts(customers);

        var debugInfo = command.GetDebugInfo();

        Assert.True(debugInfo.CommandParameterCount >= 3);
        Assert.Equal(System.Data.ConnectionState.Closed, debugInfo.ConnectionState);
    }
    #endregion

    #region executes
    /// <summary>
    /// Execute round-trip DML.
    /// </summary>
    [Fact]
    public void ExecutesToList()
    {
        var customers = Seed.Customers.ToList();

        var command = database.GetCommand().GenerateSqlServerInserts(customers);

        var result = command.ExecuteToList<Customer>();

        Assert.Equal(customers.Count, result.Count);

        Assert.Equal(customers[0].FirstName, result[0].FirstName);
        Assert.Equal(customers[1].LastName, result[1].LastName);
        Assert.Equal(customers[2].DateOfBirth, result[2].DateOfBirth);

        // TODO: move to EF tests
        //Assert.True(result.All(r => !r.IdIsNullOrEmpty()));
    }

    [Theory]
    [InlineData("SELECT * FROM dbo.Customer", "CustomerId,FirstName,LastName,DateOfBirth,Id")]
    [InlineData("SELECT 'a' A, 1 B, '2024-11-11' C", "A,B,C")]
    public void GetsReaderNames(string sql, string result)
    {
        var command = database.GetCommand().SetCommandText(sql);

        var list = command.GetReaderNames();

        Assert.Equal(result, string.Join(',', list));
    }
    #endregion

    #region executes scalar
    [Fact]
    public void GenericExecuteScalarReturnsTheFirstValue()
    {
        var id = database.GetCommand()
            .SetCommandText("SELECT TOP(1) NEWID(), 'Hello, world.';")
            .ExecuteScalar<Guid>();

        Assert.NotEqual(Guid.Empty, id);
    }

    [Fact]
    public void ExecuteScalarNullsTheDbCommand()
    {
        var command = database.GetCommand()
            .SetCommandText("SELECT * FROM Customer;");

        command.ExecuteScalar();

        Assert.Null(command.DbCommand);
    }

    [Fact]
    public void ExecuteScalarKeepsTheConnectionOpen()
    {
        var command = database.GetCommand()
            .SetCommandText("SELECT * FROM Customer;");

        command.ExecuteScalar(true);

        Assert.Equal(System.Data.ConnectionState.Open, command.DbCommand?.Connection?.State);

        command.Dispose();
    }

    [Fact]
    public void ExecuteScalarCallsPreExecuteHandler()
    {
        var handlerCalled = false;

        EventHandlers.PreExecuteHandlers.Add(_ => handlerCalled = true);

        database.GetCommand()
            .SetCommandText("SELECT 1")
            .ExecuteScalar();

        Assert.True(handlerCalled);
    }

    [Fact]
    public void ExecuteScalarCallsPostExecuteHandler()
    {
        var handlerCalled = false;

        EventHandlers.PostExecuteHandlers.Add(_ => handlerCalled = true);

        database.GetCommand()
            .SetCommandText("SELECT 1")
            .ExecuteScalar();

        Assert.True(handlerCalled);
    }

    [Fact]
    public void ExecuteScalarCallsUnhandledExceptionHandler()
    {
        var handlerCalled = false;

        EventHandlers.UnhandledExceptionHandlers.Add((_, _) =>
        {
            handlerCalled = true;
        });

        void Action() =>
            database.GetCommand()
                .SetCommandText("bogus SQL statement;")
                .ExecuteScalar();

        Assert.Throws<SqlException>(Action);
        Assert.True(handlerCalled);
    }
    #endregion

    #region inserts
    /// <summary>
    /// Test for the OUTPUT Inserted.* clause.
    /// </summary>
    [Fact]
    public void InsertsAndExecutesToObject()
    {
        var customer = TestHelpers.Map<Customer>(Seed.CustomerWithTraits);

        var command = database.GetCommand();

        command.GenerateSqlServerInsertWithOutput(customer, "Customer");

        var customerModel = command.ExecuteToObject<CustomerWithTraits>();

        Assert.NotNull(customerModel);
    }

    /// <summary>
    /// Execute round-trip DML.
    /// </summary>
    [Fact]
    public void InsertsAndExecutesToList()
    {
        using var scope = new TransactionScope();

        var customers = new List<Customer>
        {
            Seed.Customer1,
            Seed.Customer3,
            Seed.Customer2
        };

        var command = database.GetCommand()
            .GenerateSqlServerInserts(customers);
            
        var models = command.ExecuteToList<CustomerWithTraits>();

        Assert.Equal(customers[0].FirstName, models[0].FirstName);
        Assert.Equal(customers[1].LastName, models[1].LastName);
        Assert.Equal(customers[2].DateOfBirth, models[2].DateOfBirth);
    }
    #endregion

    #region aggregates
    /// <summary>
    /// Get an object out.
    /// </summary>
    [Theory]
    [InlineData(7, "Malissia", "Haxbie", "33758 Pearson Hill", "Guadalupe")]
    [InlineData(290, "Sashenka", "Trebble", "49378 Tennyson Pass", "President Roxas")]
    [InlineData(2421, "Johanna", "Salzburger", "4273 Montana Lane", "Shaami-Yurt")]
    public void GetsModel(long personId, string first, string last, string line1, string city)
    {
        var person = database.GetSelectByIdCommand("Person", personId)
            .ExecuteToObject<Person>();

        person.Address = database.GetSelectByIdCommand("Address", personId)
            .ExecuteToObject<Address>();
        
        Assert.NotNull(person);
        Assert.NotNull(person.Address);

        Assert.Equal(first, person.FirstName);
        Assert.Equal(last, person.LastName);
        Assert.Equal(line1, person.Address.Line1);
        Assert.Equal(city, person.Address?.City);
    }
    #endregion
}
