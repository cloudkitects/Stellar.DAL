using System.Data;
using System.Data.Common;
using System.Globalization;
using Stellar.DAL.Tests.Data;

namespace Stellar.DAL.Tests;

[Collection("Local Database collection")]
public class DbCommandExtensionsTests(LocalSqlDatabaseFixture fixture)
{
    readonly LocalSqlDatabaseFixture database = fixture;
    
    private const string Template = "INSERT INTO {0} ({1}) VALUES({2});";
    private static readonly char[] separator = ['\r', '\n'];

    /// <summary>
    /// Gets a <see cref="DbCommand"/> for testing purposes.
    /// </summary>
    /// <returns><see cref="DbCommand"/> instance.</returns>
    public DbCommand GetCommand()
    {
        return database.GetCommand().DbCommand;
    }

    #region AddParameters
    [Fact]
    public void AddParameterArray()
    {
        var command = GetCommand();

        var param1 = command.CreateParameter("@SuperHeroName", "Superman");
        var param2 = command.CreateParameter("@AlterEgoFirstName", "Clark");
        var param3 = command.CreateParameter("@AlterEgoLastName", "Kent");

        command = command.AddParameters(param1, param2, param3);

        Assert.Equal(param1.Value, command.Parameters[param1.ParameterName].Value);
        Assert.Equal(param2.Value, command.Parameters[param2.ParameterName].Value);
        Assert.Equal(param3.Value, command.Parameters[param3.ParameterName].Value);
    }

    [Fact]
    public void AddParameterList()
    {
        var command = GetCommand();

        var superHeroNameParameter = command.CreateParameter("@SuperHeroName", "Superman");
        var alterEgoFirstNameParameter = command.CreateParameter("@AlterEgoFirstName", "Clark");
        var alterEgoLastNameParameter = command.CreateParameter("@AlterEgoLastName", "Kent");

        var parameterList = new List<DbParameter> { superHeroNameParameter, alterEgoFirstNameParameter, alterEgoLastNameParameter };

        command = command.AddParameters(parameterList);

        Assert.Equal(superHeroNameParameter.Value, command.Parameters[superHeroNameParameter.ParameterName].Value);
        Assert.Equal(alterEgoFirstNameParameter.Value, command.Parameters[alterEgoFirstNameParameter.ParameterName].Value);
        Assert.Equal(alterEgoLastNameParameter.Value, command.Parameters[alterEgoLastNameParameter.ParameterName].Value);
    }

    [Fact]
    public void AddParameterDictionary()
    {
        var command = GetCommand();

        var superHeroName = new KeyValuePair<string, object>("@SuperHeroName", "Superman");
        var alterEgoFirstName = new KeyValuePair<string, object>("@AlterEgoFirstName", "Clark");
        var alterEgoLastName = new KeyValuePair<string, object>("@AlterEgoLastName", "Kent");

        IDictionary<string, object> dictionary = new Dictionary<string, object>
        {
            { superHeroName.Key, superHeroName.Value },
            { alterEgoFirstName.Key, alterEgoFirstName.Value },
            { alterEgoLastName.Key, alterEgoLastName.Value }
        };

        command = command.AddParameters(dictionary);

        Assert.Equal(superHeroName.Value, command.Parameters[superHeroName.Key].Value);
        Assert.Equal(alterEgoFirstName.Value, command.Parameters[alterEgoFirstName.Key].Value);
        Assert.Equal(alterEgoLastName.Value, command.Parameters[alterEgoLastName.Key].Value);
    }

    [Fact]
    public void ExpandMultivaluedParameters()
    {
        var command = GetCommand();

        command.CommandText = "SELECT * FROM SuperHero WHERE SuperHeroName IN (@SuperHeroNames)";

        const string parameterName = "@SuperHeroNames";

        const string superman = "Superman";
        const string batman = "Batman";
        const string spiderman = "Spider-Man";

        var parameterList = new List<string> { superman, batman, spiderman };

        command = command.AddParameters(parameterName, parameterList);

        Assert.Equal(superman, command.Parameters[parameterName + "_p0"]?.Value?.ToString());
        Assert.Equal(batman, command.Parameters[parameterName + "_p1"]?.Value?.ToString());
        Assert.Equal(spiderman, command.Parameters[parameterName + "_p2"]?.Value?.ToString());
    }

    [Fact]
    public void ExpandMultivaluedTypedParameters()
    {
        var command = GetCommand();

        command.CommandText = "SELECT * FROM SuperHero WHERE SuperHeroName IN (@SuperHeroNames)";

        const string parameterName = "@SuperHeroNames";

        const string superman = "Superman";
        const string batman = "Batman";
        const string spiderman = "Spider-Man";

        var parameterList = new List<string> { superman, batman, spiderman };

        command = command.AddParameters(parameterName, parameterList, DbType.AnsiString);


        Assert.Contains(parameterName, command.Parameters[0].ParameterName);
        Assert.Equal(superman, command.Parameters[0]?.Value?.ToString());
        Assert.Equal(DbType.AnsiString, command.Parameters[0].DbType);

        Assert.Contains(parameterName, command.Parameters[1].ParameterName);
        Assert.Equal(batman, command.Parameters[1]?.Value?.ToString());
        Assert.Equal(DbType.AnsiString, command.Parameters[1].DbType);

        Assert.Contains(parameterName, command.Parameters[2].ParameterName);
        Assert.Equal(spiderman, command.Parameters[2]?.Value?.ToString());
        Assert.Equal(DbType.AnsiString, command.Parameters[2].DbType);

        Assert.Contains("SELECT * FROM SuperHero WHERE SuperHeroName IN (@SuperHeroNames_p0,@SuperHeroNames_p1,@SuperHeroNames_p2)", command.CommandText);
    }

    [Fact]
    public void NullParameterListNameThrows()
    {
        var command = GetCommand();

        command.CommandText = "SELECT * FROM SuperHero WHERE SuperHeroName IN (@SuperHeroNames)";

        const string? parameterName = null; // Under test

        const string superman = "Superman";
        const string batman = "Batman";
        const string spiderman = "Spider-Man";

        var parameterList = new List<string> { superman, batman, spiderman };

        void Action() => command.AddParameters(parameterName, parameterList);

        Assert.Throws<ArgumentNullException>(Action);
    }

    [Fact]
    public void NullParameterListThrows()
    {
        var command = GetCommand();

        command.CommandText = "SELECT * FROM SuperHero WHERE SuperHeroName IN (@SuperHeroNames)";

        const string parameterName = "@SuperHeroNames";

        void Action() => command.AddParameters(parameterName, (List<string>)null);

        Assert.Throws<ArgumentNullException>(Action);
    }

    [Fact]
    public void EmptyParameterListThrows()
    {
        var command = GetCommand();

        command.CommandText = "SELECT * FROM SuperHero WHERE SuperHeroName IN (@SuperHeroNames)";

        const string parameterName = "@SuperHeroNames";

        void Action() => command.AddParameters(parameterName, new List<string>());

        Assert.Throws<Exception>(Action);
    }

    [Fact]
    public void AddingParametersBeforeSettingCommandTextThrows()
    {
        var command = GetCommand();

        const string parameterName = "@SuperHeroNames";

        const string superman = "Superman";
        const string batman = "Batman";
        const string spiderman = "Spider-Man";

        var parameterList = new List<string> { superman, batman, spiderman };

        void Action() => command.AddParameters(parameterName, parameterList);

        Assert.Throws<ArgumentException>(Action);
    }

    [Fact]
    public void ParameterNotInCommandTextThrows()
    {
        var command = GetCommand();

        command.CommandText = "SELECT * FROM SuperHero WHERE SuperHeroName IN (@SupaHeroNames)";

        const string parameterName = "@SuperHeroNames";

        const string superman = "Superman";
        const string batman = "Batman";
        const string spiderman = "Spider-Man";

        var parameterList = new List<string> { superman, batman, spiderman };

        void Action() => command.AddParameters(parameterName, parameterList);

        Assert.Throws<Exception>(Action);
    }
    #endregion

    #region CreateParameter
    [Fact]
    public void CreateParameter1()
    {
        var command = GetCommand();

        const string parameterName = "@SuperHeroName";

        object parameterValue = "Superman";

        var superHeroNameParameter = command.CreateParameter(parameterName, parameterValue);

        Assert.Equal(parameterName, superHeroNameParameter.ParameterName);
        Assert.Equal(parameterValue, superHeroNameParameter.Value);
    }

    [Fact]
    public void CreateParameter2()
    {
        var command = GetCommand();

        const string parameterName = "@SuperHeroName";

        object parameterValue = "Superman";

        const DbType dbType = DbType.AnsiString;

        var superHeroNameParameter = command.CreateParameter(parameterName, parameterValue, dbType);

        Assert.Equal(parameterName, superHeroNameParameter.ParameterName);
        Assert.Equal(parameterValue, superHeroNameParameter.Value);
        Assert.Equal(dbType, superHeroNameParameter.DbType);
    }

    [Fact]
    public void CreateParameter3()
    {
        var command = GetCommand();

        const string parameterName = "@SuperHeroName";

        object parameterValue = "Superman";

        const DbType dbType = DbType.AnsiString;

        const ParameterDirection parameterDirection = ParameterDirection.Output;

        var superHeroNameParameter = command.CreateParameter(parameterName, parameterValue, dbType, parameterDirection);

        Assert.Equal(parameterName, superHeroNameParameter.ParameterName);
        Assert.Equal(parameterValue, superHeroNameParameter.Value);
        Assert.Equal(dbType, superHeroNameParameter.DbType);
        Assert.Equal(parameterDirection, superHeroNameParameter.Direction);
    }
    #endregion

    #region AddParameter
    [Fact]
    public void AddParameter()
    {
        var command = GetCommand();
        var parameter = command.CreateParameter();

        parameter.ParameterName = "@SuperHeroName";
        parameter.Value = "Superman";
        parameter.Direction = ParameterDirection.InputOutput;

        command = command.AddParameter(parameter);

        Assert.Equal(parameter.Value, command.Parameters[parameter.ParameterName].Value);
    }

    [Fact]
    public void NullParameterThrows()
    {
        var command = GetCommand();

        void Action() => command.AddParameter(null);

        Assert.Throws<ArgumentNullException>(Action);
    }

    [Fact]
    public void AddNameAndValue()
    {
        var command = GetCommand();

        const string parameterName = "@SuperHeroName";

        object parameterValue = "Superman";

        command = command.AddParameter(parameterName, parameterValue);

        Assert.Equal(parameterValue, command.Parameters[parameterName].Value);
    }

    [Fact]
    public void NullParameterNameThrows1()
    {
        var command = GetCommand();

        const string parameterName = null;

        object parameterValue = "Superman";

        void Action() => command.AddParameter(parameterName, parameterValue);

        Assert.Throws<ArgumentNullException>(Action);
    }

    [Fact]
    public void AddNameAndValueAndType()
    {
        var command = GetCommand();

        const string parameterName = "@SuperHeroName";

        object parameterValue = "Superman";

        command = command.AddParameter(parameterName, parameterValue, DbType.AnsiString);

        Assert.Equal(parameterValue, command.Parameters[parameterName].Value);
    }

    [Fact]
    public void NullParameterNameThrows2()
    {
        var command = GetCommand();

        const string parameterName = null;

        object parameterValue = "Superman";

        void Action() => command.AddParameter(parameterName, parameterValue, DbType.AnsiString);

        Assert.Throws<ArgumentNullException>(Action);
    }
    #endregion

    #region SetCommandText
    [Fact]
    public void SetCommandText()
    {
        const string commandText = "SELECT * FROM SuperHero";

        var command = GetCommand()
            .SetCommandText(commandText);

        Assert.Equal(commandText, command.CommandText);
    }

    [Fact]
    public void SetCommandTextOverwrites()
    {
        const string commandText = "SELECT * FROM SuperHero";

        var command = GetCommand()
            .SetCommandText("Hello World!");

        command = command
            .SetCommandText(commandText);

        Assert.Equal(commandText, command.CommandText);
    }
    #endregion

    #region AppendCommandText
    [Fact]
    public void AppendWhenEmptySets()
    {
        const string commandText = "SELECT * FROM SuperHero";

        var command = GetCommand();

        command = command.AppendCommandText(commandText);

        Assert.Equal(commandText, command.CommandText);
    }

    [Fact]
    public void AppendAppends()
    {
        const string commandText1 = "SELECT * FROM SuperHero;";
        const string commandText2 = "SELECT * FROM Monsters;";

        var command = GetCommand()
            .SetCommandText(commandText1);

        command = command.AppendCommandText(commandText2);

        Assert.Equal(commandText1 + commandText2, command.CommandText);
    }
    #endregion

    #region GenerateInsertCommand
    [Fact]
    public void GenerateInsertCommandFromObjectWithFields()
    {
        var command = GetCommand()
            .GenerateInsertCommand(Seed.CustomerWithFields, Template);

        Assert.NotNull(command.CommandText);
        Assert.Contains("INSERT", command.CommandText);
    }


    [Fact]
    public void GenerateInsertCommandFromObjectWithProperties()
    {
        var command = GetCommand()
            .GenerateInsertCommand(Seed.Customer1, Template);

        Assert.NotNull(command.CommandText);
        Assert.Contains("INSERT", command.CommandText);
    }

    [Fact]
    public void GenerateInsertCommandWithTypeNameAsTableName()
    {
        var command = GetCommand()
            .GenerateInsertCommand(Seed.CustomerWithFields, Template);

        Assert.Contains("Customer", command.CommandText);
    }

    [Fact]
    public void GenerateInsertCommandWithSuppliedTableName()
    {
        var command = GetCommand()
            .GenerateInsertCommand(Seed.CustomerWithFields, Template, "[Person]");

        Assert.Contains("[Person]", command.CommandText);
    }

    [Fact]
    public void GenerateInsertCommandWithParameterNamesFromObjectFields()
    {
        var command = GetCommand()
            .GenerateInsertCommand(Seed.CustomerWithFields, Template, "[Person]");

        var parameters = command.Parameters.Cast<DbParameter>().ToList();

        Assert.Equal(Seed.CustomerWithFields.FirstName, parameters.FirstOrDefault(x => x.ParameterName.Contains("@FirstName"))?.Value?.ToString());
        Assert.Equal(Seed.CustomerWithFields.LastName, parameters.FirstOrDefault(x => x.ParameterName.Contains("@LastName"))?.Value?.ToString());
        Assert.Equal(Seed.CustomerWithFields.DateOfBirth?.ToString(CultureInfo.CurrentCulture), parameters.FirstOrDefault(x => x.ParameterName.Contains("@DateOfBirth"))?.Value?.ToString());

        Assert.Contains("@FirstName", command.CommandText);
        Assert.Contains("@LastName", command.CommandText);
        Assert.Contains("@DateOfBirth", command.CommandText);
    }

    [Fact]
    public void GenerateInsertCommandWithAnonymousObject()
    {
        var customer = new { Seed.CustomerWithFields.FirstName, Seed.CustomerWithFields.LastName, Seed.CustomerWithFields.DateOfBirth };

        var command = GetCommand();

        command = command.GenerateInsertCommand(customer, Template, "[Person]");

        Assert.NotNull(command.CommandText);
        Assert.Contains("INSERT", command.CommandText);
    }

    [Fact]
    public void GenerateInsertCommandWithAnonymousObjectAndNoTableNameThrows()
    {
        var customer = new { Seed.CustomerWithFields.FirstName, Seed.CustomerWithFields.LastName, Seed.CustomerWithFields.DateOfBirth };

        var command = GetCommand();

        void Action() => command.GenerateInsertCommand(customer, Template);

        var exception = Assert.Throws<ArgumentNullException>(Action);

        Assert.NotNull(exception);

        Console.WriteLine(exception.Message);
    }

    [Fact]
    public void GenerateInsertCommandWithNullObjectThrows()
    {
        var command = GetCommand();

        void Action() => command.GenerateInsertCommand(null, Template);

        var exception = Assert.Throws<ArgumentNullException>(Action);

        Assert.NotNull(exception);

        Console.WriteLine(exception.Message);
    }

    [Fact]
    public void GenerateInsertCommandWithNullTemplateThrows()
    {
        var customer = new { Seed.CustomerWithFields.FirstName, Seed.CustomerWithFields.LastName, Seed.CustomerWithFields.DateOfBirth };

        var command = GetCommand();

        void Action() => command.GenerateInsertCommand(customer, null, "[Customer]");

        var exception = Assert.Throws<ArgumentNullException>(Action);

        Assert.NotNull(exception);

        Console.WriteLine(exception.Message);
    }

    [Fact]
    public void GenerateInsertCommandWithEmptyTemplateThrows()
    {
        var customer = new { Seed.CustomerWithFields.FirstName, Seed.CustomerWithFields.LastName, Seed.CustomerWithFields.DateOfBirth };

        var command = GetCommand();

        void Action() => command.GenerateInsertCommand(customer, "", "[Customer]");

        var exception = Assert.Throws<ArgumentException>(Action);

        Assert.NotNull(exception);

        Console.WriteLine(exception.Message);
    }

    [Fact]
    public void GenerateInsertCommandWithInvalidTemplateThrows()
    {
        var customer = new { Seed.CustomerWithFields.FirstName, Seed.CustomerWithFields.LastName, Seed.CustomerWithFields.DateOfBirth };

        var command = GetCommand();

        void Action() => command.GenerateInsertCommand(customer, "An_Invalid_Template{0}", "[Customer]");

        var exception = Assert.Throws<Exception>(Action);

        Assert.NotNull(exception);

        Console.WriteLine(exception.Message);
    }

    [Fact]
    public void GenerateInsertCommandDoesNotEscapeTableNameByDefault()
    {
        var customer = Seed.CustomerWithFields;

        var command = GetCommand()
            .GenerateInsertCommand(customer, Template);

        Assert.Contains(" CustomerWithFields ", command.CommandText);
    }

    [Theory]
    [InlineData(KeywordEscapeMethod.SquareBracket, '[', ']')]
    [InlineData(KeywordEscapeMethod.Backtick, '`', '`')]
    [InlineData(KeywordEscapeMethod.DoubleQuote, '\"', '\"')]
    public void GenerateInsertCommandEscapesTableName(KeywordEscapeMethod keywordEscapeMethod, char prefix, char suffix)
    {
        var customer = Seed.CustomerWithFields;

        var command = GetCommand()
            .GenerateInsertCommand(customer, Template, null, keywordEscapeMethod);

        Assert.Contains($" {prefix}CustomerWithFields{suffix} ", command.CommandText);
    }

    [Theory]
    [InlineData(KeywordEscapeMethod.SquareBracket, '[', ']')]
    [InlineData(KeywordEscapeMethod.Backtick, '`', '`')]
    [InlineData(KeywordEscapeMethod.DoubleQuote, '\"', '\"')]
    public void GenerateInsertCommandEscapesColumnNames(KeywordEscapeMethod keywordEscapeMethod, char prefix, char suffix)
    {
        var customer = Seed.CustomerWithFields;

        var command = GetCommand();

        command = command.GenerateInsertCommand(customer, Template, null, keywordEscapeMethod);

        Assert.Contains($"{prefix}FirstName{suffix}", command.CommandText);
        Assert.Contains($"{prefix}LastName{suffix}", command.CommandText);
        Assert.Contains($"{prefix}DateOfBirth{suffix}", command.CommandText);
    }


    /// <remarks>
    /// The command generated won't fly
    /// </remarks>
    [Fact]
    public void GenerateInsertCommandForObjectWithoutFields()
    {
        var customer = new CustomerWithoutFields();

        var command = GetCommand();

        command = command.GenerateInsertCommand(customer, Template);

        Assert.NotNull(command.CommandText);
        Assert.Equal($"INSERT INTO {customer.GetType().Name} () VALUES();", command.CommandText);
    }
    #endregion

    #region Generate SQL Server Inserts
    [Theory]
    [InlineData("Customer")]
    [InlineData("[Customer]")]
    [InlineData(null)]
    public void GenerateInsertsForSqlServer(string table)
    {
        var customers = Seed.Customers.ToList();

        var command = GetCommand()
            .GenerateSqlServerInserts(customers, table);

        Assert.NotNull(command.CommandText);
        Assert.Contains("INSERT", command.CommandText);
        Assert.Contains("OUTPUT Inserted.*", command.CommandText);
        Assert.Matches(" [[]?Customer", command.CommandText);
        Assert.Equal(customers.Count, command.CommandText.Split(separator, StringSplitOptions.RemoveEmptyEntries).Length);
    }
    #endregion

    #region SetCommandTimeout
    [Fact]
    public void SetCommandTimeout()
    {
        const int commandTimeoutInSeconds = 60;

        var command = GetCommand()
            .SetCommandTimeout(commandTimeoutInSeconds);

        Assert.Equal(commandTimeoutInSeconds, command.CommandTimeout);
    }
    #endregion

    #region SetCommandType
    [Theory]
    [InlineData(CommandType.Text)]
    [InlineData(CommandType.StoredProcedure)]
    [InlineData(CommandType.TableDirect, Skip = "not supported by the .Net Framework SqlClient Data Provider.")]
    public void SetCommandType(CommandType commandType)
    {
        var command = GetCommand()
            .SetCommandType(commandType);

        Assert.Equal(commandType, command.CommandType);
    }
    #endregion
}
