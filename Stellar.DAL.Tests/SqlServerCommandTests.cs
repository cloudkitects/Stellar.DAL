using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Transactions;

using NUnit.Framework;
using Stellar.DAL.Model;
using Stellar.DAL.Tests.Data;

namespace Stellar.DAL.Tests
{
    [TestFixture]
    public class SqlServerCommandTests : DatabaseIntegrationTests
    {
        public SqlServerCommandTests()
        {
            GetCommand()
                .SetCommandText(@$"BULK INSERT Person FROM '{Directory.GetCurrentDirectory()}\Data\persons2.tsv';")
                .ExecuteNonQuery();
            
            GetCommand()
                .SetCommandText(@$"BULK INSERT [Address] FROM '{Directory.GetCurrentDirectory()}\Data\Addresses.tsv';")
                .ExecuteNonQuery();
        }

        #region GetDebugCommandObject
        [Test]
        public void GetDebugCommandText()
        {
            var customers = Seed.Customers.ToList();

            var command = GetCommand().GenerateInsertsForSqlServer(customers);

            var debugInfo = command.GetDebugInfo();

            Assert.That(debugInfo.CommandParameterCount >= 3);
            Assert.AreEqual(ConnectionState.Closed, debugInfo.ConnectionState);
        }
        #endregion

        #region Execute
        /// <summary>
        /// Execute round-trip DML.
        /// </summary>
        [Test]
        public void ExecuteToList()
        {
            var customers = Seed.Customers.ToList();

            var command = GetCommand().GenerateInsertsForSqlServer(customers);

            var result = command.ExecuteToList<Customer>();

            Assert.AreEqual(customers.Count, result.Count);

            Assert.AreEqual(customers[0].FirstName, result[0].FirstName);
            Assert.AreEqual(customers[1].LastName, result[1].LastName);
            Assert.AreEqual(customers[2].DateOfBirth, result[2].DateOfBirth);

            Assert.That(result.All(r => !r.IdIsNullOrEmpty()));
        }
        #endregion

        #region ExecuteScalar
        [Test]
        public void GenericExecuteScalarReturnsTheFirstValue()
        {
            var id = GetCommand()
                .SetCommandText("SELECT TOP(1) Id, FirstName FROM Customer;")
                .ExecuteScalar<Guid>();

            Assert.NotNull(id);
        }

        [Test]
        public void ExecuteScalarNullsTheDbCommand()
        {
            var command = GetCommand()
                .SetCommandText("SELECT * FROM Customer;");

            command.ExecuteScalar();

            Assert.IsNull(command.DbCommand);
        }

        [Test]
        public void ExecuteScalarKeepsTheConnectionOpen()
        {
            var command = GetCommand()
                .SetCommandText("SELECT * FROM Customer;");

            command.ExecuteScalar(true);

            Assert.That(command.DbCommand.Connection.State == ConnectionState.Open);

            command.Dispose();
        }

        [Test]
        public void ExecuteScalarCallsPreExecuteHandler()
        {
            var handlerCalled = false;

            EventHandlers.DatabaseCommandPreExecuteEventHandlers.Add(_ => handlerCalled = true);

            GetCommand()
                .SetCommandText("SELECT 1")
                .ExecuteScalar();

            Assert.IsTrue(handlerCalled);
        }

        [Test]
        public void ExecuteScalarCallsPostExecuteHandler()
        {
            var handlerCalled = false;

            EventHandlers.DatabaseCommandPostExecuteEventHandlers.Add(_ => handlerCalled = true);

            GetCommand()
                .SetCommandText("SELECT 1")
                .ExecuteScalar();

            Assert.IsTrue(handlerCalled);
        }

        [Test]
        public void ExecuteScalarCallsUnhandledExceptionHandler()
        {
            var handlerCalled = false;

            EventHandlers.DatabaseCommandUnhandledExceptionEventHandlers.Add((_, _) =>
            {
                handlerCalled = true;
            });

            void Action() =>
                GetCommand()
                    .SetCommandText("bogus SQL statement;")
                    .ExecuteScalar();

            Assert.Throws<SqlException>(Action);
            Assert.IsTrue(handlerCalled);
        }
        #endregion

        #region Generate Insert
        /// <summary>
        /// Test for the OUTPUT Inserted.* clause.
        /// </summary>
        [Test]
        public void InsertAndExecuteToObject()
        {
            var customer = TestHelpers.Map<Customer>(Seed.CustomerWithTraits);

            var command = GetCommand();

            command.GenerateInsertForSqlServer(customer, "Customer");

            var customerModel = command.ExecuteToObject<CustomerModel>();

            Assert.That(customerModel != null);
        }

        /// <summary>
        /// Execute round-trip DML.
        /// </summary>
        [Test]
        public void InsertAndExecuteToList()
        {
            using var scope = new TransactionScope();

            var customers = new List<Customer>
            {
                Seed.Customer1,
                Seed.Customer3,
                Seed.Customer2
            };

            var command = GetCommand()
                .GenerateInsertsForSqlServer(customers);
                
            var models = command.ExecuteToList<CustomerModel>();

            Assert.AreEqual(customers[0].FirstName, models[0].FirstName);
            Assert.AreEqual(customers[1].LastName, models[1].LastName);
            Assert.AreEqual(customers[2].DateOfBirth, models[2].DateOfBirth);
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
        [TestCaseSource(typeof(Seed), nameof(Seed.Persons)), Parallelizable]
        public void SaveModelBottomUp1(Person person)
        {
            var addressId = SavedId(person.Address);

            var rowsAffected = Save(person
                .ExtendWith("AddressId", addressId));

            Assert.That(rowsAffected > 0);
        }

        /// <summary>
        /// Save a model from the bottom up (components first)
        /// using fluent syntax and getting it back with
        /// composite Ids.
        /// </summary>
        [TestCaseSource(typeof(Seed), nameof(Seed.Persons0)), Parallelizable]
        public void SaveModelBottomUp2(Person person)
        {
            var result = GetInsertCommand(person
                    .ExtendWith("AddressId", SavedId(person.Address)))
                .ExecuteToObject<Person>();

            result.Address = person.Address;

            Assert.NotNull(result.Id);
            Assert.NotNull(result.Address.Id);
            
            Assert.AreEqual(person.FirstName, result.FirstName);
            Assert.AreEqual(person.LastName, result.LastName);
            Assert.AreEqual(person.Email, result.Email);
            Assert.AreEqual(person.Phone, result.Phone);
            Assert.AreEqual(person.Address.City, result.Address.City);
        }

        /// <summary>
        /// Save a model from the top down (root first)
        /// using fluent syntax and getting it back.
        /// </summary>
        [TestCaseSource(typeof(Seed), nameof(Seed.Persons1)), Parallelizable]
        public void SaveModelTopDown(Person model)
        {
            var person = TestHelpers.Map<Person>(model);

            var personModel = GetInsertCommand(person)
                .ExecuteToObject<Person>();

            personModel.Address = model.Address;
            
            var addressId = SavedId(model.Address);

            var id = GetCommand()
                .SetCommandText("UPDATE Person SET AddressId = @AddressId OUTPUT Inserted.Id WHERE PersonId = @PersonId")
                .AddParameter("@AddressId", addressId)
                .AddParameter(@"PersonId", personModel.PersonId)
                .ExecuteScalar<Guid>();

            Assert.NotNull(id);
            Assert.AreEqual(model.LastName, personModel.LastName);
            Assert.AreEqual(model.Email, personModel.Email);
            Assert.AreEqual(model.Phone, personModel.Phone);
            Assert.AreEqual(model.Address.City, personModel.Address.City);
        }

        /// <summary>
        /// Get an object out.
        /// </summary>
        [Test]
        public void GetModel([Random(1L, 2500L, 2500)] long personId)
        {
            var personModel = GetSelectByIdCommand("Person", personId)
                .ExecuteToObject<Person>();

            personModel.Address = GetSelectByIdCommand("Address", personId)
                .ExecuteToObject<Address>();
            
            Assert.NotNull(personModel);
            Assert.NotNull(personModel.Address);
        }

        [Test]
        public void Delete()
        {

        }
        #endregion
    }
}
