using System.Data;
using System.Data.SqlClient;
using System.Transactions;

using Stellar.DAL.Model;
using Stellar.DAL.Tests.Data;

namespace Stellar.DAL.Tests
{
    [Collection("Local Database collection")]
    public class SqlServerCommandTests
    {
        readonly LocalDatabaseFixture database;

        public SqlServerCommandTests(LocalDatabaseFixture fixture)
        {
            database = fixture;
        }

        #region GetDebugCommandObject
        [Fact]
        public void GetDebugCommandText()
        {
            var customers = Seed.Customers.ToList();

            var command = database.GetCommand().GenerateInsertsForSqlServer(customers);

            var debugInfo = command.GetDebugInfo();

            Assert.True(debugInfo.CommandParameterCount >= 3);
            Assert.Equal(ConnectionState.Closed, debugInfo.ConnectionState);
        }
        #endregion

        #region Execute
        /// <summary>
        /// Execute round-trip DML.
        /// </summary>
        [Fact]
        public void ExecuteToList()
        {
            var customers = Seed.Customers.ToList();

            var command = database.GetCommand().GenerateInsertsForSqlServer(customers);

            var result = command.ExecuteToList<Customer>();

            Assert.Equal(customers.Count, result.Count);

            Assert.Equal(customers[0].FirstName, result[0].FirstName);
            Assert.Equal(customers[1].LastName, result[1].LastName);
            Assert.Equal(customers[2].DateOfBirth, result[2].DateOfBirth);

            Assert.True(result.All(r => !r.IdIsNullOrEmpty()));
        }
        #endregion

        #region ExecuteScalar
        [Fact]
        public void GenericExecuteScalarReturnsTheFirstValue()
        {
            var id = database.GetCommand()
                .SetCommandText("SELECT TOP(1) Id, FirstName FROM Customer;")
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

            Assert.Equal(ConnectionState.Open, command.DbCommand?.Connection?.State);

            command.Dispose();
        }

        [Fact]
        public void ExecuteScalarCallsPreExecuteHandler()
        {
            var handlerCalled = false;

            EventHandlers.DatabaseCommandPreExecuteEventHandlers.Add(_ => handlerCalled = true);

            database.GetCommand()
                .SetCommandText("SELECT 1")
                .ExecuteScalar();

            Assert.True(handlerCalled);
        }

        [Fact]
        public void ExecuteScalarCallsPostExecuteHandler()
        {
            var handlerCalled = false;

            EventHandlers.DatabaseCommandPostExecuteEventHandlers.Add(_ => handlerCalled = true);

            database.GetCommand()
                .SetCommandText("SELECT 1")
                .ExecuteScalar();

            Assert.True(handlerCalled);
        }

        [Fact]
        public void ExecuteScalarCallsUnhandledExceptionHandler()
        {
            var handlerCalled = false;

            EventHandlers.DatabaseCommandUnhandledExceptionEventHandlers.Add((_, _) =>
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

        #region Generate Insert
        /// <summary>
        /// Test for the OUTPUT Inserted.* clause.
        /// </summary>
        [Fact]
        public void InsertAndExecuteToObject()
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
        public void InsertAndExecuteToList()
        {
            using var scope = new TransactionScope();

            var customers = new List<Customer>
            {
                Seed.Customer1,
                Seed.Customer3,
                Seed.Customer2
            };

            var command = database.GetCommand()
                .GenerateInsertsForSqlServer(customers);
                
            var models = command.ExecuteToList<CustomerWithTraits>();

            Assert.Equal(customers[0].FirstName, models[0].FirstName);
            Assert.Equal(customers[1].LastName, models[1].LastName);
            Assert.Equal(customers[2].DateOfBirth, models[2].DateOfBirth);
        }
        #endregion

        #region Aggregates
        /// <summary>
        /// Save a model from the bottom up (components first)
        /// using fluent syntax and without care for the
        /// return value beyond passing the test.
        /// </summary>
        /// <remarks>
        /// There is no question that the OUTPUT clause (just like the request/response pattern) violates Command-Query-Separation,
        /// but persisting the state literally changes (or at least extends) the state in a relational database, e.g., by adding relations.
        /// CQS compliance would be to not care for the return value (remove the clause and execute a non-query command), e.g., Save(model).
        /// "Light" CQS returns new objects, e.g., newModel = Save(model), but unless model is read-only, anyone can model = Save(model).
        ///
        /// CQS does not apply to REST APIs: POST, PUT, and DELETE verbs imply the intention to change the state, yet HTTP
        /// demands returning proper responses, thus violating "pure" CQS, even if it only responds "OK".
        /// </remarks>
        [Theory]
        [ClassData(typeof(Seed.Persons))]
        public void SaveModelBottomUp1(Person person)
        {
            var addressId = database.SavedId(person.Address);

            var rowsAffected = database.Insert(person.ExtendWith("AddressId", addressId));

            Assert.True(rowsAffected > 0);
        }

        /// <summary>
        /// Save a model from the bottom up (components first)
        /// using fluent syntax and getting it back with
        /// composite Ids.
        /// </summary>
        [Theory]
        [ClassData(typeof(Seed.Persons0))]
        public void SaveModelBottomUp2(Person person)
        {
            var result = database.GetInsertCommand(person
                    .ExtendWith("AddressId", database.SavedId(person.Address)))
                .ExecuteToObject<Person>();

            // sad state of affairs if we cannot encapsulate composition.
            result.Address = person.Address;

            Assert.Equal(person.FirstName, result.FirstName);
            Assert.Equal(person.LastName, result.LastName);
            Assert.Equal(person.Email, result.Email);
            Assert.Equal(person.Phone, result.Phone);
            Assert.Equal(person.Address?.City, result.Address?.City);
        }

        /// <summary>
        /// Save a model from the top down (root first)
        /// using fluent syntax and getting it back.
        /// </summary>
        [Theory]
        [ClassData(typeof(Seed.Persons1))]
        public void SaveModelTopDown(Person model)
        {
            var person = TestHelpers.Map<Person>(model);

            var personModel = database.GetInsertCommand(person)
                .ExecuteToObject<Person>();

            personModel.Address = model.Address;
            
            var addressId = database.SavedId(model.Address);

            _ = database.GetCommand()
                .SetCommandText("UPDATE Person SET AddressId = @AddressId OUTPUT Inserted.Id WHERE PersonId = @PersonId")
                .AddParameter("@AddressId", addressId)
                .AddParameter(@"PersonId", personModel.PersonId)
                .ExecuteScalar<Guid>();

            Assert.Equal(model.LastName, personModel.LastName);
            Assert.Equal(model.Email, personModel.Email);
            Assert.Equal(model.Phone, personModel.Phone);
            Assert.Equal(model.Address?.City, personModel.Address?.City);
        }

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

        //[Fact]
        //public void Delete()
        //{

        //}
        #endregion
    }
}
