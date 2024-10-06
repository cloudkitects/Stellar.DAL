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

    #region select script generators
    public static DatabaseCommand GenerateSqlServerSelectById(this DatabaseCommand databaseCommand, string table, long id)
    {
        databaseCommand.DbCommand.GenerateSqlServerSelectById(table, id);

        return databaseCommand;
    }
    #endregion

    #region execute wrappers
    public static long ExecuteNonQuery(this DatabaseCommand databaseCommand, bool keepConnectionOpen = false)
    {
        long numberOfRowsAffected;

        try
        {
            EventHandlers.InvokePreExecuteEventHandlers(databaseCommand);

            databaseCommand.DbCommand.OpenConnection();

            numberOfRowsAffected = databaseCommand.DbCommand.ExecuteNonQuery();

            EventHandlers.InvokePostExecuteEventHandlers(databaseCommand);
        }
        catch (Exception exception)
        {
            EventHandlers.InvokeUnhandledExceptionEventHandlers(exception, databaseCommand);

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

    public static object ExecuteScalar(this DatabaseCommand databaseCommand, bool keepConnectionOpen = false)
    {
        object returnValue;

        try
        {
            EventHandlers.InvokePreExecuteEventHandlers(databaseCommand);

            databaseCommand.DbCommand.OpenConnection();

            returnValue = databaseCommand.DbCommand.ExecuteScalar();

            if (returnValue == DBNull.Value)
            {
                returnValue = null;
            }

            EventHandlers.InvokePostExecuteEventHandlers(databaseCommand);
        }
        catch (Exception exception)
        {
            EventHandlers.InvokeUnhandledExceptionEventHandlers(exception, databaseCommand);

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

    public static T ExecuteScalar<T>(this DatabaseCommand databaseCommand, bool keepConnectionOpen = false)
    {
        var returnValue = databaseCommand.ExecuteScalar(keepConnectionOpen);

        return returnValue.ConvertTo<T>();
    }

    public static void ExecuteReader(this DatabaseCommand databaseCommand, Action<IDataRecord> dataRecordCallback, bool keepConnectionOpen = false)
    {
        try
        {
            EventHandlers.InvokePreExecuteEventHandlers(databaseCommand);

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

            EventHandlers.InvokePostExecuteEventHandlers(databaseCommand);
        }
        catch (Exception exception)
        {
            EventHandlers.InvokeUnhandledExceptionEventHandlers(exception, databaseCommand);

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

    public static List<T> ExecuteToList<T>(this DatabaseCommand databaseCommand, Func<IDataRecord, T> mapper, bool keepConnectionOpen = false)
    {
        var list = new List<T>();

        databaseCommand.ExecuteReader(callback =>
        {
            list.Add(mapper.Invoke(callback));
        }, keepConnectionOpen);

        return list;
    }

    public static List<T> ExecuteToList<T>(this DatabaseCommand databaseCommand, bool keepConnectionOpen = false)
    {
        return databaseCommand.ExecuteToList(ToObject<T>, keepConnectionOpen);
    }

    public static T ExecuteToObject<T>(this DatabaseCommand databaseCommand, bool keepConnectionOpen = false) where T : new()
    {
        return databaseCommand.ExecuteToList<T>(keepConnectionOpen).FirstOrDefault();
    }

    public static List<dynamic> ExecuteToDynamicList(this DatabaseCommand databaseCommand, bool keepConnectionOpen = false)
    {
        return databaseCommand.ExecuteToList(ToDynamic, keepConnectionOpen);
    }

    public static dynamic ExecuteToDynamic(this DatabaseCommand databaseCommand, bool keepConnectionOpen = false)
    {
        return databaseCommand.ExecuteToDynamicList(keepConnectionOpen).FirstOrDefault();
    }

    public static DataSet ExecuteToDataSet(this DatabaseCommand databaseCommand, bool keepConnectionOpen = false)
    {
        var dataSet = new DataSet();

        try
        {
            EventHandlers.InvokePreExecuteEventHandlers(databaseCommand);

            databaseCommand.DbCommand.OpenConnection();

            var dbProviderFactory = DbProviderFactories.GetFactory(databaseCommand.DbCommand.Connection);

            var dataAdapter = dbProviderFactory.CreateDataAdapter() ??
                throw new Exception("An unexpected null was returned from a call to DbProviderFactory.CreateDataAdapter().");
            
            dataAdapter.SelectCommand = databaseCommand.DbCommand;

            dataAdapter.Fill(dataSet);

            EventHandlers.InvokePostExecuteEventHandlers(databaseCommand);
        }
        catch (Exception exception)
        {
            EventHandlers.InvokeUnhandledExceptionEventHandlers(exception, databaseCommand);

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

    #region miscellaneous
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
