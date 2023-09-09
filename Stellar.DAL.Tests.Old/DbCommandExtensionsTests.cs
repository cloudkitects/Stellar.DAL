using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Stellar.DAL.Tests.Data;
using NUnit.Framework;

namespace Stellar.DAL.Tests
{
    [TestFixture]
    public class DbCommandExtensionsTests
    {
        private const string Template = "INSERT INTO {0} ({1}) VALUES({2});";

        /// <summary>
        /// Gets a <see cref="DbCommand"/> for testing purposes.
        /// </summary>
        /// <returns><see cref="DbCommand"/> instance.</returns>
        public DbCommand GetCommand()
        {
            return DatabaseClient.GetCommand(@"Data Source=(LocalDb)\MSSQLLocalDB;Integrated Security=True;").DbCommand;
        }

        #region AddParameters
        [Test]
        public void AddParameterArray()
        {
            var command = GetCommand();

            var superHeroNameParameter = command.CreateParameter("@SuperHeroName", "Superman");
            var alterEgoFirstNameParameter = command.CreateParameter("@AlterEgoFirstName", "Clark");
            var alterEgoLastNameParameter = command.CreateParameter("@AlterEgoLastName", "Kent");

            command = command.AddParameters(superHeroNameParameter, alterEgoFirstNameParameter, alterEgoLastNameParameter);

            Assert.That(command.Parameters[superHeroNameParameter.ParameterName].Value == superHeroNameParameter.Value);
            Assert.That(command.Parameters[alterEgoFirstNameParameter.ParameterName].Value == alterEgoFirstNameParameter.Value);
            Assert.That(command.Parameters[alterEgoLastNameParameter.ParameterName].Value == alterEgoLastNameParameter.Value);
        }

        [Test]
        public void AddParameterList()
        {
            var command = GetCommand();

            var superHeroNameParameter = command.CreateParameter("@SuperHeroName", "Superman");
            var alterEgoFirstNameParameter = command.CreateParameter("@AlterEgoFirstName", "Clark");
            var alterEgoLastNameParameter = command.CreateParameter("@AlterEgoLastName", "Kent");

            var parameterList = new List<DbParameter> { superHeroNameParameter, alterEgoFirstNameParameter, alterEgoLastNameParameter };

            command = command.AddParameters(parameterList);

            Assert.That(command.Parameters[superHeroNameParameter.ParameterName].Value == superHeroNameParameter.Value);
            Assert.That(command.Parameters[alterEgoFirstNameParameter.ParameterName].Value == alterEgoFirstNameParameter.Value);
            Assert.That(command.Parameters[alterEgoLastNameParameter.ParameterName].Value == alterEgoLastNameParameter.Value);
        }

        [Test]
        public void AddParameterDictionary()
        {
            var command = GetCommand();

            var superHeroName = new KeyValuePair<string, object>("@SuperHeroName", "Superman");
            var alterEgoFirstName = new KeyValuePair<string, object>("@AlterEgoFirstName", "Clark");
            var alterEgoLastName = new KeyValuePair<string, object>("@AlterEgoLastName", "Kent");

            IDictionary<string, object> dictionary = new Dictionary<string, object>();

            dictionary.Add(superHeroName);
            dictionary.Add(alterEgoFirstName);
            dictionary.Add(alterEgoLastName);

            command = command.AddParameters(dictionary);

            Assert.That(command.Parameters[superHeroName.Key].Value == superHeroName.Value);
            Assert.That(command.Parameters[alterEgoFirstName.Key].Value == alterEgoFirstName.Value);
            Assert.That(command.Parameters[alterEgoLastName.Key].Value == alterEgoLastName.Value);
        }

        [Test]
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

            Assert.That(command.Parameters[parameterName + "_p0"].Value.ToString() == superman);
            Assert.That(command.Parameters[parameterName + "_p1"].Value.ToString() == batman);
            Assert.That(command.Parameters[parameterName + "_p2"].Value.ToString() == spiderman);
        }

        [Test]
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


            Assert.That(command.Parameters[0].ParameterName.Contains(parameterName));
            Assert.That(command.Parameters[0].Value.ToString() == superman);
            Assert.That(command.Parameters[0].DbType == DbType.AnsiString);

            Assert.That(command.Parameters[1].ParameterName.Contains(parameterName));
            Assert.That(command.Parameters[1].Value.ToString() == batman);
            Assert.That(command.Parameters[1].DbType == DbType.AnsiString);

            Assert.That(command.Parameters[2].ParameterName.Contains(parameterName));
            Assert.That(command.Parameters[2].Value.ToString() == spiderman);
            Assert.That(command.Parameters[2].DbType == DbType.AnsiString);

            Assert.That(command.CommandText.Contains("SELECT * FROM SuperHero WHERE SuperHeroName IN (@SuperHeroNames_p0,@SuperHeroNames_p1,@SuperHeroNames_p2)"));
        }

        [Test]
        public void NullParameterListNameThrows()
        {
            var command = GetCommand();

            command.CommandText = "SELECT * FROM SuperHero WHERE SuperHeroName IN (@SuperHeroNames)";

            const string parameterName = null; // Under test

            const string superman = "Superman";
            const string batman = "Batman";
            const string spiderman = "Spider-Man";

            var parameterList = new List<string> { superman, batman, spiderman };

            void Action() => command.AddParameters(parameterName, parameterList);

            Assert.Throws<ArgumentNullException>(Action);
        }

        [Test]
        public void NullParameterListThrows()
        {
            var command = GetCommand();

            command.CommandText = "SELECT * FROM SuperHero WHERE SuperHeroName IN (@SuperHeroNames)";

            const string parameterName = "@SuperHeroNames";

            void Action() => command.AddParameters(parameterName, (List<string>)null);

            Assert.Throws<ArgumentNullException>(Action);
        }

        [Test]
        public void EmptyParameterListThrows()
        {
            var command = GetCommand();

            command.CommandText = "SELECT * FROM SuperHero WHERE SuperHeroName IN (@SuperHeroNames)";

            const string parameterName = "@SuperHeroNames";

            void Action() => command.AddParameters(parameterName, new List<string>());

            Assert.Catch<Exception>(Action);
        }

        [Test]
        public void AddingParametersBeforeSettingCommandTextThrows()
        {
            var command = GetCommand();

            const string parameterName = "@SuperHeroNames";

            const string superman = "Superman";
            const string batman = "Batman";
            const string spiderman = "Spider-Man";

            var parameterList = new List<string> { superman, batman, spiderman };

            void Action() => command.AddParameters(parameterName, parameterList);

            Assert.Catch<Exception>(Action);
        }

        [Test]
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

            Assert.Catch<Exception>(Action);
        }
        #endregion

        #region CreateParameter
        [Test]
        public void CreateParameter1()
        {
            var command = GetCommand();

            const string parameterName = "@SuperHeroName";

            object parameterValue = "Superman";

            var superHeroNameParameter = command.CreateParameter(parameterName, parameterValue);

            Assert.That(superHeroNameParameter.ParameterName == parameterName);
            Assert.That(superHeroNameParameter.Value == parameterValue);
        }

        [Test]
        public void CreateParameter2()
        {
            var command = GetCommand();

            const string parameterName = "@SuperHeroName";

            object parameterValue = "Superman";

            const DbType dbType = DbType.AnsiString;

            var superHeroNameParameter = command.CreateParameter(parameterName, parameterValue, dbType);

            Assert.That(superHeroNameParameter.ParameterName == parameterName);
            Assert.That(superHeroNameParameter.Value == parameterValue);
            Assert.That(superHeroNameParameter.DbType == dbType);
        }

        [Test]
        public void CreateParameter3()
        {
            var command = GetCommand();

            const string parameterName = "@SuperHeroName";

            object parameterValue = "Superman";

            const DbType dbType = DbType.AnsiString;

            const ParameterDirection parameterDirection = ParameterDirection.Output;

            var superHeroNameParameter = command.CreateParameter(parameterName, parameterValue, dbType, parameterDirection);

            Assert.That(superHeroNameParameter.ParameterName == parameterName);
            Assert.That(superHeroNameParameter.Value == parameterValue);
            Assert.That(superHeroNameParameter.DbType == dbType);
            Assert.That(superHeroNameParameter.Direction == parameterDirection);
        }
        #endregion

        #region AddParameter
        [Test]
        public void AddParameter()
        {
            var command = GetCommand();
            var parameter = command.CreateParameter();

            parameter.ParameterName = "@SuperHeroName";
            parameter.Value = "Superman";
            parameter.Direction = ParameterDirection.InputOutput;

            command = command.AddParameter(parameter);

            Assert.That(command.Parameters[parameter.ParameterName].Value == parameter.Value);
        }

        [Test]
        public void NullParameterThrows()
        {
            var command = GetCommand();

            void Action() => command.AddParameter(null);

            Assert.Throws<ArgumentNullException>(Action);
        }

        [Test]
        public void AddNameAndValue()
        {
            var command = GetCommand();

            const string parameterName = "@SuperHeroName";

            object parameterValue = "Superman";

            command = command.AddParameter(parameterName, parameterValue);

            Assert.That(command.Parameters[parameterName].Value == parameterValue);
        }

        [Test]
        public void NullParameterNameThrows1()
        {
            var command = GetCommand();

            const string parameterName = null;

            object parameterValue = "Superman";

            void Action() => command.AddParameter(parameterName, parameterValue);

            Assert.Throws<ArgumentNullException>(Action);
        }

        [Test]
        public void AddNameAndValueAndType()
        {
            var command = GetCommand();

            const string parameterName = "@SuperHeroName";

            object parameterValue = "Superman";

            command = command.AddParameter(parameterName, parameterValue, DbType.AnsiString);

            Assert.That(command.Parameters[parameterName].Value == parameterValue);
        }

        [Test]
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
        [Test]
        public void SetCommandText()
        {
            const string commandText = "SELECT * FROM SuperHero";

            var command = GetCommand()
                .SetCommandText(commandText);

            Assert.That(command.CommandText == commandText);
        }

        [Test]
        public void SetCommandTextOverwrites()
        {
            const string commandText = "SELECT * FROM SuperHero";

            var command = GetCommand()
                .SetCommandText("Hello World!");

            command = command
                .SetCommandText(commandText);

            Assert.That(command.CommandText == commandText);
        }
        #endregion

        #region AppendCommandText
        [Test]
        public void AppendWhenEmptySets()
        {
            const string commandText = "SELECT * FROM SuperHero";

            var command = GetCommand();

            command = command.AppendCommandText(commandText);

            Assert.That(command.CommandText == commandText);
        }

        [Test]
        public void AppendAppends()
        {
            const string commandText1 = "SELECT * FROM SuperHero;";
            const string commandText2 = "SELECT * FROM Monsters;";

            var command = GetCommand()
                .SetCommandText(commandText1);

            command = command.AppendCommandText(commandText2);

            Assert.That(command.CommandText == commandText1 + commandText2);
        }
        #endregion

        #region GenerateInsertCommand
        [Test]
        public void GenerateInsertCommandFromObjectWithFields()
        {
            var command = GetCommand()
                .GenerateInsertCommand(Seed.SCustomerWithFields, Template);

            Assert.NotNull(command.CommandText);
            Assert.That(command.CommandText.Contains("INSERT"));
        }


        [Test]
        public void GenerateInsertCommandFromObjectWithProperties()
        {
            var command = GetCommand()
                .GenerateInsertCommand(Seed.Customer1, Template);

            Assert.NotNull(command.CommandText);
            Assert.That(command.CommandText.Contains("INSERT"));
        }

        [Test]
        public void GenerateInsertCommandWithTypeNameAsTableName()
        {
            var command = GetCommand()
                .GenerateInsertCommand(Seed.SCustomerWithFields, Template);

            Assert.That(command.CommandText.Contains("Customer"));
        }

        [Test]
        public void GenerateInsertCommandWithSuppliedTableName()
        {
            var command = GetCommand()
                .GenerateInsertCommand(Seed.SCustomerWithFields, Template, "[Person]");

            Assert.That(command.CommandText.Contains("[Person]"));
        }

        [Test]
        public void GenerateInsertCommandWithParameterNamesFromObjectFields()
        {
            var command = GetCommand()
                .GenerateInsertCommand(Seed.SCustomerWithFields, Template, "[Person]");

            var parameters = command.Parameters.Cast<DbParameter>().ToList();

            Assert.That(parameters.FirstOrDefault(x => x.ParameterName.Contains("@FirstName"))?.Value.ToString() == Seed.SCustomerWithFields.FirstName);
            Assert.That(parameters.FirstOrDefault(x => x.ParameterName.Contains("@LastName"))?.Value.ToString() == Seed.SCustomerWithFields.LastName);
            Assert.That(parameters.FirstOrDefault(x => x.ParameterName.Contains("@DateOfBirth"))?.Value.ToString() == Seed.SCustomerWithFields.DateOfBirth.ToString(CultureInfo.CurrentCulture));

            Assert.That(command.CommandText.Contains("@FirstName"));
            Assert.That(command.CommandText.Contains("@LastName"));
            Assert.That(command.CommandText.Contains("@DateOfBirth"));
        }

        [Test]
        public void GenerateInsertCommandWithAnonymousObject()
        {
            var customer = new { Seed.SCustomerWithFields.FirstName, Seed.SCustomerWithFields.LastName, Seed.SCustomerWithFields.DateOfBirth };

            var command = GetCommand();

            command = command.GenerateInsertCommand(customer, Template, "[Person]");

            Assert.NotNull(command.CommandText);
            Assert.That(command.CommandText.Contains("INSERT"));
        }

        [Test]
        public void GenerateInsertCommandWithAnonymousObjectAndNoTableNameThrows()
        {
            var customer = new { Seed.SCustomerWithFields.FirstName, Seed.SCustomerWithFields.LastName, Seed.SCustomerWithFields.DateOfBirth };

            var command = GetCommand();

            void Action() => command.GenerateInsertCommand(customer, Template);

            var exception = Assert.Catch<ArgumentNullException>(Action);

            Assert.NotNull(exception);

            Console.WriteLine(exception.Message);
        }

        [Test]
        public void GenerateInsertCommandWithNullObjectThrows()
        {
            var command = GetCommand();

            void Action() => command.GenerateInsertCommand(null, Template);

            var exception = Assert.Catch<ArgumentNullException>(Action);

            Assert.NotNull(exception);

            Console.WriteLine(exception.Message);
        }

        [Test]
        public void GenerateInsertCommandWithNullTemplateThrows()
        {
            var customer = new { Seed.SCustomerWithFields.FirstName, Seed.SCustomerWithFields.LastName, Seed.SCustomerWithFields.DateOfBirth };

            var command = GetCommand();

            void Action() => command.GenerateInsertCommand(customer, null, "[Customer]");

            var exception = Assert.Catch<ArgumentNullException>(Action);

            Assert.NotNull(exception);

            Console.WriteLine(exception.Message);
        }

        [Test]
        public void GenerateInsertCommandWithEmptyTemplateThrows()
        {
            var customer = new { Seed.SCustomerWithFields.FirstName, Seed.SCustomerWithFields.LastName, Seed.SCustomerWithFields.DateOfBirth };

            var command = GetCommand();

            void Action() => command.GenerateInsertCommand(customer, "", "[Customer]");

            var exception = Assert.Catch<ArgumentNullException>(Action);

            Assert.NotNull(exception);

            Console.WriteLine(exception.Message);
        }

        [Test]
        public void GenerateInsertCommandWithInvalidTemplateThrows()
        {
            var customer = new { Seed.SCustomerWithFields.FirstName, Seed.SCustomerWithFields.LastName, Seed.SCustomerWithFields.DateOfBirth };

            var command = GetCommand();

            void Action() => command.GenerateInsertCommand(customer, "An_Invalid_Template{0}", "[Customer]");

            var exception = Assert.Catch<Exception>(Action);

            Assert.NotNull(exception);

            Console.WriteLine(exception.Message);
        }

        [Test]
        public void GenerateInsertCommandDoesNotEscapeTableNameByDefault()
        {
            var customer = Seed.SCustomerWithFields;

            var command = GetCommand()
                .GenerateInsertCommand(customer, Template);

            Assert.That(command.CommandText.Contains($" {customer.GetType().Name} "));
        }

        [TestCase(KeywordEscapeMethod.SquareBracket, '[', ']')]
        [TestCase(KeywordEscapeMethod.Backtick, '`', '`')]
        [TestCase(KeywordEscapeMethod.DoubleQuote, '\"', '\"')]
        public void GenerateInsertCommandEscapesTableName(KeywordEscapeMethod keywordEscapeMethod, char prefix, char suffix)
        {
            var customer = Seed.SCustomerWithFields;

            var command = GetCommand()
                .GenerateInsertCommand(customer, Template, null, keywordEscapeMethod);

            Assert.That(command.CommandText.Contains($" {prefix}{customer.GetType().Name}{suffix} "));
        }

        [TestCase(KeywordEscapeMethod.SquareBracket, '[', ']')]
        [TestCase(KeywordEscapeMethod.Backtick, '`', '`')]
        [TestCase(KeywordEscapeMethod.DoubleQuote, '\"', '\"')]
        public void GenerateInsertCommandEscapesColumnNames(KeywordEscapeMethod keywordEscapeMethod, char prefix, char suffix)
        {
            var customer = Seed.SCustomerWithFields;

            var command = GetCommand();

            command = command.GenerateInsertCommand(customer, Template, null, keywordEscapeMethod);

            Assert.That(command.CommandText.Contains($"{prefix}FirstName{suffix}"));
            Assert.That(command.CommandText.Contains($"{prefix}LastName{suffix}"));
            Assert.That(command.CommandText.Contains($"{prefix}DateOfBirth{suffix}"));
        }

        [Test(Description = "Typical DDD test--the command is generated but it won't fly :)")]
        public void GenerateInsertCommandForObjectWithoutFields()
        {
            var customer = new CustomerWithoutFields();

            var command = GetCommand();

            command = command.GenerateInsertCommand(customer, Template);

            Assert.NotNull(command.CommandText);
            Assert.That(command.CommandText == $"INSERT INTO {customer.GetType().Name} () VALUES();");
        }
        #endregion

        #region Generate SQL Server Inserts
        [TestCase("Customer")]
        [TestCase("[Customer]")]
        [TestCase(null)]
        public void GenerateInsertsForSqlServer(string table)
        {
            var customers = Seed.Customers.ToList();

            var command = GetCommand()
                .GenerateInsertsForSqlServer(customers, table);

            Assert.NotNull(command.CommandText);
            Assert.That(command.CommandText.Contains("INSERT"));
            Assert.That(command.CommandText.Contains("OUTPUT Inserted.*"));
            Assert.That(Regex.IsMatch(command.CommandText, " [[]?Customer"));
            Assert.That(command.CommandText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length == customers.Count);
        }
        #endregion

        #region SetCommandTimeout
        [Test]
        public void SetCommandTimeout()
        {
            const int commandTimeoutInSeconds = 60;

            var command = GetCommand()
                .SetCommandTimeout(commandTimeoutInSeconds);

            Assert.That(command.CommandTimeout == commandTimeoutInSeconds);
        }
        #endregion

        #region SetCommandType
        [TestCase(CommandType.Text)]
        [TestCase(CommandType.StoredProcedure)]
        [TestCase(CommandType.TableDirect, IgnoreReason = "not supported by the .Net Framework SqlClient Data Provider.")]
        public void SetCommandType(CommandType commandType)
        {
            var command = GetCommand()
                .SetCommandType(commandType);

            Assert.That(command.CommandType == commandType);
        }
        #endregion
    }
}
