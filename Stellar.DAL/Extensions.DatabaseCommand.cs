using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net;

namespace Stellar.DAL;

/// <summary>
/// <see cref="DatabaseCommand" /> fluent extensions.
/// </summary>
public static partial class Extensions
{
    #region setters
    public static DatabaseCommand SetCommandText(this DatabaseCommand databaseCommand, string commandText)
    {
        databaseCommand.DbCommand.SetCommandText(commandText);

        return databaseCommand;
    }

    public static DatabaseCommand AppendCommandText(this DatabaseCommand databaseCommand, string commandText)
    {
        databaseCommand.DbCommand.AppendCommandText(commandText);

        return databaseCommand;
    }

    public static DatabaseCommand SetCommandType(this DatabaseCommand databaseCommand, CommandType commandType)
    {
        databaseCommand.DbCommand.SetCommandType(commandType);

        return databaseCommand;
    }

    public static DatabaseCommand SetCommandTimeout(this DatabaseCommand databaseCommand, int commandTimeoutSeconds)
    {
        databaseCommand.DbCommand.SetCommandTimeout(commandTimeoutSeconds);

        return databaseCommand;
    }

    public static DatabaseCommand SetTransaction(this DatabaseCommand databaseCommand, DbTransaction dbTransaction)
    {
        databaseCommand.DbCommand.SetTransaction(dbTransaction);

        return databaseCommand;
    }
    #endregion

    #region parameter helpers
    public static DatabaseCommand AddParameter(this DatabaseCommand databaseCommand, DbParameter dbParameter)
    {
        databaseCommand.DbCommand.AddParameter(dbParameter);

        return databaseCommand;
    }

    public static DatabaseCommand AddParameter(this DatabaseCommand databaseCommand, string parameterName, object parameterValue)
    {
        databaseCommand.DbCommand.AddParameter(parameterName, parameterValue);

        return databaseCommand;
    }

    public static DatabaseCommand AddParameter(this DatabaseCommand databaseCommand, string parameterName, object parameterValue, DbType dbType)
    {
        databaseCommand.DbCommand.AddParameter(parameterName, parameterValue, dbType);

        return databaseCommand;
    }

    public static DatabaseCommand AddParameters(this DatabaseCommand databaseCommand, IEnumerable<DbParameter> dbParameters)
    {
        databaseCommand.DbCommand.AddParameters(dbParameters);

        return databaseCommand;
    }

    public static DatabaseCommand AddParameters(this DatabaseCommand databaseCommand, params DbParameter[] dbParameters)
    {
        databaseCommand.DbCommand.AddParameters(dbParameters);

        return databaseCommand;
    }

    public static DatabaseCommand AddParameters(this DatabaseCommand databaseCommand, IDictionary<string, object> parameterNameAndValueDictionary)
    {
        databaseCommand.DbCommand.AddParameters(parameterNameAndValueDictionary);

        return databaseCommand;
    }

    public static DatabaseCommand AddParameters<T>(this DatabaseCommand databaseCommand, string parameterName, List<T> parameterValues)
    {
        databaseCommand.DbCommand.AddParameters(parameterName, parameterValues);

        return databaseCommand;
    }

    public static DatabaseCommand AddParameters<T>(this DatabaseCommand databaseCommand, string parameterName, List<T> parameterValues, DbType dbType)
    {
        databaseCommand.DbCommand.AddParameters(parameterName, parameterValues, dbType);

        return databaseCommand;
    }

    public static DbParameter CreateParameter(this DatabaseCommand databaseCommand, string parameterName, object parameterValue)
    {
        var parameter = databaseCommand.DbCommand.CreateParameter(parameterName, parameterValue);

        return parameter;
    }

    public static DbParameter CreateParameter(this DatabaseCommand databaseCommand, string parameterName, object parameterValue, DbType dbType)
    {
        var parameter = databaseCommand.DbCommand.CreateParameter(parameterName, parameterValue, dbType);

        return parameter;
    }

    public static DbParameter CreateParameter(this DatabaseCommand databaseCommand, string parameterName, object parameterValue, DbType dbType, ParameterDirection parameterDirection)
    {
        var parameter = databaseCommand.DbCommand.CreateParameter(parameterName, parameterValue, dbType, parameterDirection);

        return parameter;
    }
    #endregion

    #region insert script generators
    
    #region MySQL
    public static DatabaseCommand GenerateMySqlInsert(this DatabaseCommand databaseCommand, object item, string table = null)
    {
        databaseCommand.DbCommand.GenerateMySqlInsert(item, table);

        return databaseCommand;
    }

    public static DatabaseCommand GenerateMySqlInserts<T>(this DatabaseCommand databaseCommand, List<T> list, string table = null)
    {
        databaseCommand.DbCommand.GenerateMySqlInserts(list, table);

        return databaseCommand;
    }
    #endregion

    #region SqLite
    public static DatabaseCommand GenerateSQLiteInsert(this DatabaseCommand databaseCommand, object item, string table = null)
    {
        databaseCommand.DbCommand.GenerateSQLiteInsert(item, table);

        return databaseCommand;
    }

    public static DatabaseCommand GenerateSQLiteInserts<T>(this DatabaseCommand databaseCommand, List<T> list, string table = null)
    {
        databaseCommand.DbCommand.GenerateSQLiteInserts(list, table);

        return databaseCommand;
    }
    #endregion

    #region SqlServer
    public static DatabaseCommand GenerateSqlServerInsert(this DatabaseCommand databaseCommand, object item, string table = null)
    {
        databaseCommand.DbCommand.GenerateSqlServerInsert(item, table);

        return databaseCommand;
    }

    public static DatabaseCommand GenerateSqlServerInsertWithOutput(this DatabaseCommand databaseCommand, object item, string table = null)
    {
        databaseCommand.DbCommand.GenerateSqlServerInsertWithOutput(item, table);

        return databaseCommand;
    }

    public static DatabaseCommand GenerateSqlServerInserts<T>(this DatabaseCommand databaseCommand, List<T> list, string table = null)
    {
        databaseCommand.DbCommand.GenerateSqlServerInserts(list, table);

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
    public static DatabaseCommand GenerateSqlServerSelectById(this DatabaseCommand databaseCommand, string table, long id)
    {
        databaseCommand.DbCommand.GenerateSqlServerSelectById(table, id);

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
    public static List<T> ExecuteToList<T>(this DatabaseCommand databaseCommand, Func<IDataRecord, T> mapper, bool keepConnectionOpen = false)
    {
        var list = new List<T>();

        databaseCommand.ExecuteReader(callback =>
        {
            list.Add(mapper.Invoke(callback));
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
        return databaseCommand.ExecuteToList(ToObject<T>, keepConnectionOpen);
    }

    /// <summary>
    /// Executes a statement against a database and maps the results to an object of type <typeparamref name="T" />
    /// using a given <paramref name="mapper" /> function.
    /// </summary>
    /// <typeparam name="T">The type to map the results to.</typeparam>
    /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
    /// <param name="mapper">
    /// A method that takes an <see cref="IDataRecord" /> as an argument and returns an instance of type
    /// <typeparamref name="T" />.
    /// </param>
    /// <param name="keepConnectionOpen">Optional parameter indicating whether to keep the connection open. Default is false.</param>
    /// <returns>An object of type <typeparamref name="T" />.</returns>
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
        return databaseCommand.ExecuteToList(ToDynamic, keepConnectionOpen);
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

    #region transactions
    public static DbTransaction BeginTransaction(this DatabaseCommand databaseCommand)
    {
        var transaction = databaseCommand.DbCommand.BeginTransaction();

        return transaction;
    }

    public static DbTransaction BeginTransaction(this DatabaseCommand databaseCommand, IsolationLevel isolationLevel)
    {
        var transaction = databaseCommand.DbCommand.BeginTransaction(isolationLevel);

        return transaction;
    }
    #endregion

    #region Miscellaneous
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

    public static DbCommand ToDbCommand(this DatabaseCommand databaseCommand)
    {
        ArgumentNullException.ThrowIfNull(databaseCommand);

        return databaseCommand.DbCommand;
    }

    /// <summary>
    /// Generate a DTO with command and connection information.
    /// </summary>
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
