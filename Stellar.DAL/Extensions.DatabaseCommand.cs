using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net;

namespace Stellar.DAL
{
    public static partial class Extensions
    {
        #region Set properties
        /// <summary>Sets the text command to run against the data source.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="commandText">The text command to run against the data source.</param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        public static DatabaseCommand SetCommandText(this DatabaseCommand databaseCommand, string commandText)
        {
            databaseCommand.DbCommand.SetCommandText(commandText);

            return databaseCommand;
        }

        /// <summary>Appends to the text command to run against the data source.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="commandText">Text command to append to the text command to run against the data source.</param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        public static DatabaseCommand AppendCommandText(this DatabaseCommand databaseCommand, string commandText)
        {
            databaseCommand.DbCommand.AppendCommandText(commandText);

            return databaseCommand;
        }

        /// <summary>Sets the CommandType.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="commandType">CommandType which specifies how a command string is interpreted.</param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        public static DatabaseCommand SetCommandType(this DatabaseCommand databaseCommand, CommandType commandType)
        {
            databaseCommand.DbCommand.SetCommandType(commandType);

            return databaseCommand;
        }

        /// <summary>
        /// Sets the time in seconds to wait for the command to execute before throwing an exception. The default is 30 seconds.
        /// </summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="commandTimeoutSeconds">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        public static DatabaseCommand SetCommandTimeout(this DatabaseCommand databaseCommand, int commandTimeoutSeconds)
        {
            databaseCommand.DbCommand.SetCommandTimeout(commandTimeoutSeconds);

            return databaseCommand;
        }
        #endregion

        #region Parameters
        /// <summary>Adds a <see cref="DbParameter" /> to the <see cref="DatabaseCommand" />.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="dbParameter"><see cref="DbParameter" /> to add.</param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        public static DatabaseCommand AddParameter(this DatabaseCommand databaseCommand, DbParameter dbParameter)
        {
            databaseCommand.DbCommand.AddParameter(dbParameter);

            return databaseCommand;
        }

        /// <summary>Adds a parameter to the <see cref="DatabaseCommand" />.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="parameterValue">Parameter value.</param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        public static DatabaseCommand AddParameter(this DatabaseCommand databaseCommand, string parameterName, object parameterValue)
        {
            databaseCommand.DbCommand.AddParameter(parameterName, parameterValue);

            return databaseCommand;
        }

        /// <summary>Adds a parameter to the <see cref="DatabaseCommand" />.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="parameterValue">Parameter value.</param>
        /// <param name="dbType">Parameter type.</param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        public static DatabaseCommand AddParameter(this DatabaseCommand databaseCommand, string parameterName, object parameterValue, DbType dbType)
        {
            databaseCommand.DbCommand.AddParameter(parameterName, parameterValue, dbType);

            return databaseCommand;
        }

        /// <summary>Adds a list of <see cref="DbParameter" />s to the <see cref="DatabaseCommand" />.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="dbParameters">List of database parameters.</param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        public static DatabaseCommand AddParameters(this DatabaseCommand databaseCommand, IEnumerable<DbParameter> dbParameters)
        {
            databaseCommand.DbCommand.AddParameters(dbParameters);

            return databaseCommand;
        }

        /// <summary>Adds a parameter array of <see cref="DbParameter" />s to the <see cref="DatabaseCommand" />.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="dbParameters">Parameter array of database parameters.</param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        public static DatabaseCommand AddParameters(this DatabaseCommand databaseCommand, params DbParameter[] dbParameters)
        {
            databaseCommand.DbCommand.AddParameters(dbParameters);

            return databaseCommand;
        }

        /// <summary>Adds a dictionary of parameter names and values to the <see cref="DatabaseCommand" />.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="parameterNameAndValueDictionary">Dictionary of parameter names and values.</param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        public static DatabaseCommand AddParameters(this DatabaseCommand databaseCommand, IDictionary<string, object> parameterNameAndValueDictionary)
        {
            databaseCommand.DbCommand.AddParameters(parameterNameAndValueDictionary);

            return databaseCommand;
        }

        /// <summary>
        /// Adds the list of parameter values to the <see cref="DatabaseCommand" /> by replacing the given parameterName in the
        /// CommandText with a comma delimited list of generated parameter names such as "parameterName0, parameterName1,
        /// parameterName2", etc.
        /// </summary>
        /// <typeparam name="T">Parameter type.</typeparam>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="parameterValues">Parameter values.</param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="parameterName" /> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="parameterValues" /> parameter is null.</exception>
        /// <exception cref="Exception">Thrown when the <paramref name="parameterValues" /> list is empty.</exception>
        /// <exception cref="Exception">Thrown when the CommandText has not been set prior to calling this method.</exception>
        /// <exception cref="Exception">Thrown when the CommandText does not contain the <paramref name="parameterName" />.</exception>
        public static DatabaseCommand AddParameters<T>(this DatabaseCommand databaseCommand, string parameterName, List<T> parameterValues)
        {
            databaseCommand.DbCommand.AddParameters(parameterName, parameterValues);

            return databaseCommand;
        }

        /// <summary>
        /// Adds the list of parameter values of the specified <see cref="DbType" /> to the <see cref="DatabaseCommand" /> by
        /// replacing the given parameterName in the CommandText with a comma delimited list of generated parameter names such as
        /// "parameterName0, parameterName1, parameterName2", etc.
        /// </summary>
        /// <typeparam name="T">Parameter type.</typeparam>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="parameterValues">Parameter values.</param>
        /// <param name="dbType">Parameter type.</param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="parameterName" /> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="parameterValues" /> parameter is null.</exception>
        /// <exception cref="Exception">Thrown when the <paramref name="parameterValues" /> list is empty.</exception>
        /// <exception cref="Exception">Thrown when the CommandText has not been set prior to calling this method.</exception>
        /// <exception cref="Exception">Thrown when the CommandText does not contain the <paramref name="parameterName" />.</exception>
        public static DatabaseCommand AddParameters<T>(this DatabaseCommand databaseCommand, string parameterName, List<T> parameterValues, DbType dbType)
        {
            databaseCommand.DbCommand.AddParameters(parameterName, parameterValues, dbType);

            return databaseCommand;
        }

        /// <summary>Creates a <see cref="DbParameter" />.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="parameterValue">Parameter value.</param>
        /// <returns><see cref="DbParameter" />.</returns>
        public static DbParameter CreateParameter(this DatabaseCommand databaseCommand, string parameterName, object parameterValue)
        {
            var parameter = databaseCommand.DbCommand.CreateParameter(parameterName, parameterValue);

            return parameter;
        }

        /// <summary>Creates a <see cref="DbParameter" /> with a given <see cref="DbType" />.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="parameterValue">Parameter value.</param>
        /// <param name="dbType">Parameter type.</param>
        /// <returns><see cref="DbParameter" />.</returns>
        public static DbParameter CreateParameter(this DatabaseCommand databaseCommand, string parameterName, object parameterValue, DbType dbType)
        {
            var parameter = databaseCommand.DbCommand.CreateParameter(parameterName, parameterValue, dbType);

            return parameter;
        }

        /// <summary>
        /// Creates a <see cref="DbParameter" /> with a given <see cref="DbType" /> and <see cref="ParameterDirection" />.
        /// </summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="parameterValue">Parameter value.</param>
        /// <param name="dbType">Parameter type.</param>
        /// <param name="parameterDirection">Parameter direction.</param>
        /// <returns><see cref="DbParameter" />.</returns>
        public static DbParameter CreateParameter(this DatabaseCommand databaseCommand, string parameterName, object parameterValue, DbType dbType, ParameterDirection parameterDirection)
        {
            var parameter = databaseCommand.DbCommand.CreateParameter(parameterName, parameterValue, dbType, parameterDirection);

            return parameter;
        }
        #endregion

        #region Transactions
        /// <summary>Sets the transaction associated with the command.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="dbTransaction">The transaction to associate with the command.</param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        public static DatabaseCommand SetTransaction(this DatabaseCommand databaseCommand, DbTransaction dbTransaction)
        {
            databaseCommand.DbCommand.SetTransaction(dbTransaction);

            return databaseCommand;
        }

        /// <summary>
        /// Starts a database transaction and associates it with the <see cref="DatabaseCommand"/> instance.
        /// </summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <returns>An object representing the new transaction.</returns>
        public static DbTransaction BeginTransaction(this DatabaseCommand databaseCommand)
        {
            var transaction = databaseCommand.DbCommand.BeginTransaction();

            return transaction;
        }

        /// <summary>
        /// Starts a database transaction with the specified isolation level and associates it with the <see cref="DatabaseCommand"/> instance.
        /// </summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="isolationLevel">Specifies the isolation level for the transaction.</param>
        /// <returns>An object representing the new transaction.</returns>
        public static DbTransaction BeginTransaction(this DatabaseCommand databaseCommand, IsolationLevel isolationLevel)
        {
            var transaction = databaseCommand.DbCommand.BeginTransaction(isolationLevel);

            return transaction;
        }
        #endregion

        #region Generate inserts
        #region MySQL
        /// <summary>
        /// Generates a parameterized MySQL INSERT statement from the given object and adds it to the
        /// <see cref="DatabaseCommand" />.
        /// <para>
        /// Note that the generated query also selects the last inserted id using SELECT LAST_INSERT_ID() function.
        /// </para>
        /// </summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="item">Object to generate the SQL INSERT statement from.</param>
        /// <param name="table">Optional table name to insert into. If none is supplied, it will use the type name.</param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// The value of 'table' cannot be null when the object passed is an anonymous
        /// type.
        /// </exception>
        public static DatabaseCommand GenerateInsertForMySql(this DatabaseCommand databaseCommand, object item, string table = null)
        {
            databaseCommand.DbCommand.GenerateInsertForMySql(item, table);

            return databaseCommand;
        }

        /// <summary>
        /// Generates a list of concatenated parameterized MySQL INSERT statements from the given list of objects and adds it to
        /// the <see cref="DatabaseCommand" />.
        /// <para>
        /// Note that the generated query also selects the last inserted id using SELECT LAST_INSERT_ID() function.
        /// </para>
        /// </summary>
        /// <typeparam name="T">Type of the objects in the list.</typeparam>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="list">List of objects to generate the SQL INSERT statements from.</param>
        /// <param name="table">
        /// Optional table name to insert into. If none is supplied, it will use the type name. Note that this parameter is
        /// required when passing in an anonymous object or an <see cref="ArgumentNullException" /> will be thrown.
        /// </param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// The value of 'table' cannot be null when the object passed is an anonymous
        /// type.
        /// </exception>
        public static DatabaseCommand GenerateInsertsForMySql<T>(this DatabaseCommand databaseCommand, List<T> list, string table = null)
        {
            databaseCommand.DbCommand.GenerateInsertsForMySql(list, table);

            return databaseCommand;
        }
        #endregion

        #region SqLite
        /// <summary>
        /// Generates a parameterized SQLite INSERT statement from the given object and adds it to the
        /// <see cref="DatabaseCommand" />.
        /// <para>
        /// Note that the generated query also selects the last inserted id using SQLite's SELECT last_insert_rowid() function.
        /// </para>
        /// </summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="item">Object to generate the SQL INSERT statement from.</param>
        /// <param name="table">Optional table name to insert into. If none is supplied, it will use the type name.</param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// The value of 'table' cannot be null when the object passed is an anonymous
        /// type.
        /// </exception>
        // ReSharper disable once InconsistentNaming
        public static DatabaseCommand GenerateInsertForSQLite(this DatabaseCommand databaseCommand, object item, string table = null)
        {
            databaseCommand.DbCommand.GenerateInsertForSQLite(item, table);

            return databaseCommand;
        }

        /// <summary>
        /// Generates a list of concatenated parameterized SQLite INSERT statements from the given list of objects and adds it to
        /// the <see cref="DatabaseCommand" />.
        /// <para>
        /// Note that the generated query also selects the last inserted id using SQLite's SELECT last_insert_rowid() function.
        /// </para>
        /// </summary>
        /// <typeparam name="T">Type of the objects in the list.</typeparam>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="list">List of objects to generate the SQL INSERT statements from.</param>
        /// <param name="table">
        /// Optional table name to insert into. If none is supplied, it will use the type name. Note that this parameter is
        /// required when passing in an anonymous object or an <see cref="ArgumentNullException" /> will be thrown.
        /// </param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// The value of 'table' cannot be null when the object passed is an anonymous
        /// type.
        /// </exception>
        // ReSharper disable once InconsistentNaming
        public static DatabaseCommand GenerateInsertsForSQLite<T>(this DatabaseCommand databaseCommand, List<T> list, string table = null)
        {
            databaseCommand.DbCommand.GenerateInsertsForSQLite(list, table);

            return databaseCommand;
        }
        #endregion

        #region SqlServer
        /// <summary>
        /// Generates a parameterized SQL Server INSERT statement from the given object and adds it to the
        /// <see cref="DatabaseCommand" />.
        /// </summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="item">Object to generate the SQL INSERT statement from.</param>
        /// <param name="table">Optional table name to insert into. If none is supplied, it will use the type name.</param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        public static DatabaseCommand GenerateInsertForSqlServer(this DatabaseCommand databaseCommand, object item, string table = null)
        {
            databaseCommand.DbCommand.GenerateInsertForSqlServer(item, table);

            return databaseCommand;
        }

        /// <summary>
        /// Generates a list of concatenated parameterized SQL Server INSERT statements from the given list of objects and adds it
        /// to the <see cref="DatabaseCommand" />.
        /// <para>
        /// Note that the generated query also selects the last inserted id using SQL Server's SELECT SCOPE_IDENTITY() function.
        /// </para>
        /// </summary>
        /// <typeparam name="T">Type of the objects in the list.</typeparam>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="list">List of objects to generate the SQL INSERT statements from.</param>
        /// <param name="table">
        /// Optional table name to insert into. If none is supplied, it will use the type name. Note that this parameter is
        /// required when passing in an anonymous object or an <see cref="ArgumentNullException" /> will be thrown.
        /// </param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// The value of 'table' cannot be null when the object passed is an anonymous
        /// type.
        /// </exception>
        public static DatabaseCommand GenerateInsertsForSqlServer<T>(this DatabaseCommand databaseCommand, List<T> list, string table = null)
        {
            databaseCommand.DbCommand.GenerateInsertsForSqlServer(list, table);

            return databaseCommand;
        }
        #endregion
        #endregion

        #region Generate select
        /// <summary>
        /// Generates a parameterized SQL Server SELECT statement from the given object and adds it to the
        /// <see cref="DatabaseCommand" />.
        /// </summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="table">Optional table name to insert into. If none is supplied, it will use the type name.</param>
        /// <param name="id"></param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// The value of 'table' cannot be null when the object passed is an anonymous type.
        /// </exception>
        public static DatabaseCommand GenerateSelectByIdForSqlServer(this DatabaseCommand databaseCommand, string table, long id)
        {
            databaseCommand.DbCommand.GenerateSelectByIdForSqlServer(table, id);

            return databaseCommand;
        }
        #endregion

        #region Execute methods
        /// <summary>Executes a statement against the database and returns the number of rows affected.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="keepConnectionOpen">Optional parameter indicating whether to keep the connection open. Default is false.</param>
        /// <returns>The number of rows affected.</returns>
        /// <exception cref="Exception">Unexpected exception.</exception>
        public static long ExecuteNonQuery(this DatabaseCommand databaseCommand, bool keepConnectionOpen = false)
        {
            int numberOfRowsAffected;

            try
            {
                EventHandlers.InvokeDatabaseCommandPreExecuteEventHandlers(databaseCommand);

                databaseCommand.DbCommand.OpenConnection();

                numberOfRowsAffected = databaseCommand.DbCommand.ExecuteNonQuery();

                EventHandlers.InvokeDatabaseCommandPostExecuteEventHandlers(databaseCommand);
            }
            catch (Exception exception)
            {
                EventHandlers.InvokeDatabaseCommandUnhandledExceptionEventHandlers(exception, databaseCommand);

                throw;
            }
            finally
            {
                if (keepConnectionOpen == false)
                {
                    databaseCommand.DbCommand.CloseAndDispose();

                    databaseCommand.DbCommand = null;
                }
            }

            return numberOfRowsAffected;
        }

        /// <summary>
        /// Executes the query and returns the first column of the first row in the result set returned by the query. All other
        /// columns and rows are ignored.
        /// </summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="keepConnectionOpen">Optional parameter indicating whether to keep the connection open. Default is false.</param>
        /// <returns>The first column of the first row in the result set.</returns>
        public static object ExecuteScalar(this DatabaseCommand databaseCommand, bool keepConnectionOpen = false)
        {
            object returnValue;

            try
            {
                EventHandlers.InvokeDatabaseCommandPreExecuteEventHandlers(databaseCommand);

                databaseCommand.DbCommand.OpenConnection();

                returnValue = databaseCommand.DbCommand.ExecuteScalar();

                if (returnValue == DBNull.Value)
                {
                    returnValue = null;
                }

                EventHandlers.InvokeDatabaseCommandPostExecuteEventHandlers(databaseCommand);
            }
            catch (Exception exception)
            {
                EventHandlers.InvokeDatabaseCommandUnhandledExceptionEventHandlers(exception, databaseCommand);

                throw;
            }
            finally
            {
                if (!keepConnectionOpen)
                {
                    databaseCommand.DbCommand.CloseAndDispose();

                    databaseCommand.DbCommand = null;
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Executes the query and returns the first column of the first row in the result set returned by the query. All other
        /// columns and rows are ignored.
        /// </summary>
        /// <typeparam name="T">Type to convert the result to.</typeparam>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="keepConnectionOpen">Optional parameter indicating whether to keep the connection open. Default is false.</param>
        /// <returns>
        /// The first column of the first row in the result set converted to a type of <typeparamref name="T" />.
        /// </returns>
        /// <exception cref="TypeConversionException">
        /// Thrown when an error occurs attempting to convert a value to an
        /// enum.
        /// </exception>
        /// <exception cref="TypeConversionException">
        /// Thrown when an error occurs attempting to convert a value to a
        /// type.
        /// </exception>
        public static T ExecuteScalar<T>(this DatabaseCommand databaseCommand, bool keepConnectionOpen = false)
        {
            var returnValue = databaseCommand.ExecuteScalar(keepConnectionOpen);

            return returnValue.ConvertTo<T>();
        }

        /// <summary>
        /// Executes a statement against the database and calls the <paramref name="dataRecordCallback" /> action for each record
        /// returned.
        /// </summary>
        /// <remarks>
        /// For safety the DbDataReader is returned as an IDataRecord to the callback so that callers cannot modify the current row
        /// being read.
        /// </remarks>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="dataRecordCallback">Action called for each record returned.</param>
        /// <param name="keepConnectionOpen">Optional parameter indicating whether to keep the connection open. Default is false.</param>
        public static void ExecuteReader(this DatabaseCommand databaseCommand, Action<IDataRecord> dataRecordCallback, bool keepConnectionOpen = false)
        {
            try
            {
                EventHandlers.InvokeDatabaseCommandPreExecuteEventHandlers(databaseCommand);

                databaseCommand.DbCommand.OpenConnection();

                using (var dbDataReader = databaseCommand.DbCommand.ExecuteReader())
                {
                    while (dbDataReader.HasRows)
                    {
                        while (dbDataReader.Read())
                        {
                            dataRecordCallback.Invoke(dbDataReader);
                        }

                        dbDataReader.NextResult();
                    }
                }

                EventHandlers.InvokeDatabaseCommandPostExecuteEventHandlers(databaseCommand);
            }
            catch (Exception exception)
            {
                EventHandlers.InvokeDatabaseCommandUnhandledExceptionEventHandlers(exception, databaseCommand);

                throw;
            }
            finally
            {
                if (!keepConnectionOpen)
                {
                    databaseCommand.DbCommand.CloseAndDispose();
                    databaseCommand.DbCommand = null;
                }
            }
        }

        /// <summary>
        /// Executes a statement against a database and maps the results to a list of type <typeparamref name="T" /> using a given
        /// mapper function supplied to the <paramref name="mapper" /> parameter.
        /// </summary>
        /// <typeparam name="T">The type to map the results to.</typeparam>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="mapper">
        /// A method that takes an <see cref="IDataRecord" /> as an argument and returns an instance of type
        /// <typeparamref name="T" />.
        /// </param>
        /// <param name="keepConnectionOpen">Optional parameter indicating whether to keep the connection open. Default is false.</param>
        /// <returns>Results mapped to a list of type <typeparamref name="T" />.</returns>
        public static List<T> ExecuteToMap<T>(this DatabaseCommand databaseCommand, Func<IDataRecord, T> mapper, bool keepConnectionOpen = false)
        {
            var list = new List<T>();

            databaseCommand.ExecuteReader(reader =>
            {
                var mappedObject = mapper.Invoke(reader);

                list.Add(mappedObject);
            }, keepConnectionOpen);

            return list;
        }

        /// <summary>
        /// Executes a statement against a database and maps matching column names to a list of type <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type to map the results to.</typeparam>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="keepConnectionOpen">Optional parameter indicating whether to keep the connection open. Default is false.</param>
        /// <returns>Results mapped to a list of type <typeparamref name="T" />.</returns>
        public static List<T> ExecuteToList<T>(this DatabaseCommand databaseCommand, bool keepConnectionOpen = false)
        {
            return databaseCommand.ExecuteToMap(DataRecordMapper.Map<T>, keepConnectionOpen);
        }

        /// <summary>
        /// Executes a statement against a database and maps matching column names to a type of <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type to map the results to.</typeparam>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="keepConnectionOpen">Optional parameter indicating whether to keep the connection open. Default is false.</param>
        /// <returns>Results mapped to a type of <typeparamref name="T" />.</returns>
        public static T ExecuteToObject<T>(this DatabaseCommand databaseCommand, bool keepConnectionOpen = false) where T : new()
        {
            return databaseCommand.ExecuteToList<T>(keepConnectionOpen).FirstOrDefault();
        }

        /// <summary>Executes a statement against a database and maps the results to a list of type dynamic.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="keepConnectionOpen">Optional parameter indicating whether to keep the connection open. Default is false.</param>
        /// <returns>Results mapped to a list of type dynamic.</returns>
        public static List<dynamic> ExecuteToDynamicList(this DatabaseCommand databaseCommand, bool keepConnectionOpen = false)
        {
            return databaseCommand.ExecuteToMap(DataRecordMapper.MapDynamic, keepConnectionOpen);
        }

        /// <summary>Executes a statement against a database and maps the result to a dynamic object.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="keepConnectionOpen">Optional parameter indicating whether to keep the connection open. Default is false.</param>
        /// <returns>Result mapped to a dynamic object.</returns>
        public static dynamic ExecuteToDynamicObject(this DatabaseCommand databaseCommand, bool keepConnectionOpen = false)
        {
            return databaseCommand.ExecuteToDynamicList(keepConnectionOpen).FirstOrDefault();
        }

        /// <summary>Executes a statement against a database and populates the results into a <see cref="DataSet" />.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="keepConnectionOpen">Optional parameter indicating whether to keep the connection open. Default is false.</param>
        /// <returns>DataSet representing an in-memory cache of the result set.</returns>
        /// <exception cref="Exception">An unexpected null was returned from a call to DbProviderFactory.CreateDataAdapter().</exception>
        public static DataSet ExecuteToDataSet(this DatabaseCommand databaseCommand, bool keepConnectionOpen = false)
        {
            var dataSet = new DataSet();

            try
            {
                EventHandlers.InvokeDatabaseCommandPreExecuteEventHandlers(databaseCommand);

                databaseCommand.DbCommand.OpenConnection();

                var dbProviderFactory = DbProviderFactories.GetFactory(databaseCommand.DbCommand.Connection);

                var dataAdapter = dbProviderFactory.CreateDataAdapter() ??
                    throw new Exception("An unexpected null was returned from a call to DbProviderFactory.CreateDataAdapter().");
                
                dataAdapter.SelectCommand = databaseCommand.DbCommand;

                dataAdapter.Fill(dataSet);

                EventHandlers.InvokeDatabaseCommandPostExecuteEventHandlers(databaseCommand);
            }
            catch (Exception exception)
            {
                EventHandlers.InvokeDatabaseCommandUnhandledExceptionEventHandlers(exception, databaseCommand);

                throw;
            }
            finally
            {
                if (keepConnectionOpen == false)
                {
                    databaseCommand.DbCommand.CloseAndDispose();

                    databaseCommand.DbCommand = null;
                }
            }

            return dataSet;
        }

        /// <summary>
        /// Executes a statement against a database and returns the first table populated in the <see cref="DataSet" />.
        /// </summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="keepConnectionOpen">Optional parameter indicating whether to keep the connection open. Default is false.</param>
        /// <returns>
        /// DataTable representing an in-memory cache of the first <see cref="DataTable" /> result set from the returned
        /// <see cref="DataSet" />.
        /// </returns>
        public static DataTable ExecuteToDataTable(this DatabaseCommand databaseCommand, bool keepConnectionOpen = false)
        {
            return databaseCommand.ExecuteToDataSet(keepConnectionOpen).Tables[0];
        }
        #endregion

        #region Miscellaneous
        /// <summary>
        /// Tests the connection to a database and returns true if a connection can be successfully opened and closed.
        /// </summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <returns>Returns true if a connection can be successfully opened and closed. </returns>
        public static bool TestConnection(this DatabaseCommand databaseCommand)
        {
            try
            {
                databaseCommand.DbCommand.Connection.Open();

                databaseCommand.DbCommand.Connection.Close();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>Returns the underlying <see cref="DbCommand" />.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <returns><see cref="DbCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="databaseCommand" /> parameter is null.</exception>
        public static DbCommand ToDbCommand(this DatabaseCommand databaseCommand)
        {
            if (databaseCommand == null)
            {
                throw new ArgumentNullException(nameof(databaseCommand));
            }

            return databaseCommand.DbCommand;
        }

        /// <summary>
        /// Generate a DTO with command and connection information.
        /// </summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <returns>JSON string with command and connection info.</returns>
        // grant access to unit tests library
        internal static DebugInfo GetDebugInfo(this DatabaseCommand databaseCommand)
        {
            var command = databaseCommand.DbCommand;

            return new DebugInfo
            {
                MachineName = Environment.MachineName,
                HostName = Dns.GetHostEntry("LocalHost").HostName,
                DataSource = command.Connection.DataSource,
                Database = command.Connection.Database,
                ConnectionString = command.Connection.ConnectionString,
                ConnectionState = command.Connection.State,
                CommandTimeout = command.CommandTimeout,
                CommandParameterCount = command.Parameters.Count,
                AnnotatedCommandText = command.AnnotatedCommandText()
            };
        }
        #endregion
    }
}
