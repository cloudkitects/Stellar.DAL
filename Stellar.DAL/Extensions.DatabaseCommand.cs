using System.Data;
using System.Data.Common;
using System.Net;

namespace Stellar.DAL;

/// <summary>
/// <see cref="DatabaseCommand" /> fluent extensions.
/// </summary>
public static partial class Extensions
{
    #region setters
    public static DatabaseCommand SetCommandText(this DatabaseCommand command, string text)
    {
        command.DbCommand.SetCommandText(text);

        return command;
    }

    public static DatabaseCommand AppendCommandText(this DatabaseCommand command, string text)
    {
        command.DbCommand.AppendCommandText(text);

        return command;
    }

    public static DatabaseCommand SetCommandType(this DatabaseCommand command, CommandType type)
    {
        command.DbCommand.SetCommandType(type);

        return command;
    }

    public static DatabaseCommand SetCommandTimeout(this DatabaseCommand command, int timeoutSeconds)
    {
        command.DbCommand.SetCommandTimeout(timeoutSeconds);

        return command;
    }

    public static DatabaseCommand SetTransaction(this DatabaseCommand command, DbTransaction transaction)
    {
        command.DbCommand.SetTransaction(transaction);

        return command;
    }
    #endregion

    #region parameter helpers
    public static DatabaseCommand AddParameter(this DatabaseCommand command, DbParameter parameter)
    {
        command.DbCommand.AddParameter(parameter);

        return command;
    }

    public static DatabaseCommand AddParameter(this DatabaseCommand command, string name, object value)
    {
        command.DbCommand.AddParameter(name, value);

        return command;
    }

    public static DatabaseCommand AddParameter(this DatabaseCommand command, string name, object value, DbType type)
    {
        command.DbCommand.AddParameter(name, value, type);

        return command;
    }

    public static DatabaseCommand AddParameters(this DatabaseCommand command, IEnumerable<DbParameter> parameters)
    {
        command.DbCommand.AddParameters(parameters);

        return command;
    }

    public static DatabaseCommand AddParameters(this DatabaseCommand command, params DbParameter[] parameters)
    {
        command.DbCommand.AddParameters(parameters);

        return command;
    }

    public static DatabaseCommand AddParameters(this DatabaseCommand command, IDictionary<string, object> parameters)
    {
        command.DbCommand.AddParameters(parameters);

        return command;
    }

    public static DatabaseCommand AddParameters<T>(this DatabaseCommand command, string name, List<T> values)
    {
        command.DbCommand.AddParameters(name, values);

        return command;
    }

    public static DatabaseCommand AddParameters<T>(this DatabaseCommand command, string name, List<T> values, DbType type)
    {
        command.DbCommand.AddParameters(name, values, type);

        return command;
    }

    public static DbParameter CreateParameter(this DatabaseCommand command, string name, object value)
    {
        var parameter = command.DbCommand.CreateParameter(name, value);

        return parameter;
    }

    public static DbParameter CreateParameter(this DatabaseCommand command, string name, object value, DbType type)
    {
        var parameter = command.DbCommand.CreateParameter(name, value, type);

        return parameter;
    }

    public static DbParameter CreateParameter(this DatabaseCommand command, string name, object value, DbType type, ParameterDirection direction)
    {
        var parameter = command.DbCommand.CreateParameter(name, value, type, direction);

        return parameter;
    }
    #endregion

    #region insert script generators

    #region MySQL
    public static DatabaseCommand GenerateMySqlInsert(this DatabaseCommand command, object item, string? table = null)
    {
        command.DbCommand.GenerateMySqlInsert(item, table);

        return command;
    }

    public static DatabaseCommand GenerateMySqlInserts<T>(this DatabaseCommand command, List<T> list, string? table = null)
    {
        command.DbCommand.GenerateMySqlInserts(list, table);

        return command;
    }
    #endregion

    #region SqLite
    public static DatabaseCommand GenerateSQLiteInsert(this DatabaseCommand command, object item, string? table = null)
    {
        command.DbCommand.GenerateSQLiteInsert(item, table);

        return command;
    }

    public static DatabaseCommand GenerateSQLiteInserts<T>(this DatabaseCommand command, List<T> list, string? table = null)
    {
        command.DbCommand.GenerateSQLiteInserts(list, table);

        return command;
    }
    #endregion

    #region SqlServer
    public static DatabaseCommand GenerateSqlServerInsert(this DatabaseCommand command, object item, string? table = null)
    {
        command.DbCommand.GenerateSqlServerInsert(item, table);

        return command;
    }

    public static DatabaseCommand GenerateSqlServerInsertWithOutput(this DatabaseCommand command, object item, string? table = null)
    {
        command.DbCommand.GenerateSqlServerInsertWithOutput(item, table);

        return command;
    }

    public static DatabaseCommand GenerateSqlServerInserts<T>(this DatabaseCommand command, List<T> list, string? table = null)
    {
        command.DbCommand.GenerateSqlServerInserts(list, table);

        return command;
    }
    #endregion

    #endregion

    #region select script generators
    public static DatabaseCommand GenerateSqlServerSelectById(this DatabaseCommand command, string table, long id)
    {
        command.DbCommand.GenerateSqlServerSelectById(table, id);

        return command;
    }
    #endregion

    #region execute wrappers
    public static long ExecuteNonQuery(this DatabaseCommand command, bool keepAlive = false)
    {
        long numberOfRowsAffected;

        try
        {
            EventHandlers.InvokePreExecuteHandlers(command);

            command.DbCommand.OpenConnection();

            numberOfRowsAffected = command.DbCommand.ExecuteNonQuery();

            EventHandlers.InvokePostExecuteHandlers(command);
        }
        catch (Exception exception)
        {
            EventHandlers.InvokeUnhandledExceptionHandlers(exception, command);

            throw;
        }
        finally
        {
            if (!keepAlive)
            {
                command.DbCommand.CloseAndDispose();
                command.DbCommand = null!;
            }
        }

        return numberOfRowsAffected;
    }

    public static object? ExecuteScalar(this DatabaseCommand command, bool keepAlive = false)
    {
        object? returnValue;

        try
        {
            EventHandlers.InvokePreExecuteHandlers(command);

            command.DbCommand.OpenConnection();

            returnValue = command.DbCommand.ExecuteScalar();

            if (returnValue == DBNull.Value)
            {
                returnValue = null;
            }

            EventHandlers.InvokePostExecuteHandlers(command);
        }
        catch (Exception exception)
        {
            EventHandlers.InvokeUnhandledExceptionHandlers(exception, command);

            throw;
        }
        finally
        {
            if (!keepAlive)
            {
                command.DbCommand.CloseAndDispose();
                command.DbCommand = null!;
            }
        }

        return returnValue;
    }

    public static T? ExecuteScalar<T>(this DatabaseCommand command, bool keepAlive = false)
    {
        var returnValue = command.ExecuteScalar(keepAlive);

        return returnValue!.ConvertTo<T>();
    }

    public static void ExecuteReader(this DatabaseCommand command, Func<IDataRecord, bool> callback, bool keepAlive = false)
    {
        try
        {
            EventHandlers.InvokePreExecuteHandlers(command);

            command.DbCommand.OpenConnection();

            using var reader = command.DbCommand.ExecuteReader();

            var success = true;

            while (reader.HasRows)
            {
                while (reader.Read())
                {
                    if (!callback.Invoke(reader))
                    {
                        success = false;
                        break;
                    }
                }

                if (!success)
                {
                    break;
                }

                reader.NextResult();
            }

            if (success)
            {
                EventHandlers.InvokePostExecuteHandlers(command);
            }
        }
        catch (Exception exception)
        {
            EventHandlers.InvokeUnhandledExceptionHandlers(exception, command);

            throw;
        }
        finally
        {
            if (!keepAlive)
            {
                command.DbCommand.CloseAndDispose();
                command.DbCommand = null!;

            }
        }
    }

    public static void ExecuteReaderSingle(this DatabaseCommand command, Action<IDataRecord> callback, bool keepAlive = false)
    {
        try
        {
            EventHandlers.InvokePreExecuteHandlers(command);

            command.DbCommand.OpenConnection();

            using var reader = command.DbCommand.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SingleRow);

            if (reader.HasRows && reader.Read())
            {
                callback.Invoke(reader);
            }

            EventHandlers.InvokePostExecuteHandlers(command);
        }
        catch (Exception exception)
        {
            EventHandlers.InvokeUnhandledExceptionHandlers(exception, command);

            throw;
        }
        finally
        {
            if (!keepAlive)
            {
                command.DbCommand.CloseAndDispose();
                command.DbCommand = null!;
            }
        }
    }

    public static List<string> GetReaderNames(this DatabaseCommand command)
    {
        var dbCommand = command.DbCommand;

        try
        {
            dbCommand.OpenConnection();

            using var reader = dbCommand.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SchemaOnly);

            return [.. Enumerable.Range(0, reader.FieldCount).Select(reader.GetName)];

        }
        catch (Exception exception)
        {
            EventHandlers.InvokeUnhandledExceptionHandlers(exception, command);

            throw;
        }
        finally
        {
            dbCommand.CloseAndDispose();
            dbCommand = null;
        }
    }

    public static List<T> ExecuteToList<T>(this DatabaseCommand command, bool keepAlive = false, Func<IDataRecord, T>? callback = null)
    {
        var list = new List<T>();

        command.ExecuteReader(record =>
        {
            try
            {
                T obj = callback is null
                    ? record.ToObject<T>()!
                    : callback.Invoke(record);

                list.Add(obj);

                return true;
            }
            catch
            {
                return false;
            }
        }, keepAlive);

        return list;
    }

    public static T? ExecuteToObject<T>(this DatabaseCommand command, bool keepAlive = false) where T : new()
    {
        T? obj = default;

        command.ExecuteReaderSingle(record =>
        {
            obj = ToObject<T>(record);
        }, keepAlive);

        return obj;
    }

    public static List<dynamic> ExecuteToDynamicList(this DatabaseCommand command, bool keepAlive = false)
    {
        var list = new List<dynamic>();

        command.ExecuteReader(record =>
        {
            try
            {
                list.Add(record.ToDynamic());

                return true;
            }
            catch
            {
                return false;
            }

        }, keepAlive);

        return list;
    }

    public static dynamic? ExecuteToDynamic(this DatabaseCommand command, bool keepAlive = false)
    {
        dynamic? obj = default;

        command.ExecuteReaderSingle(record =>
        {
            obj = ToDynamic(record);
        }, keepAlive);

        return obj;
    }

    public static DataSet ExecuteToDataSet(this DatabaseCommand command, bool keepAlive = false)
    {
        var dataSet = new DataSet();

        try
        {
            EventHandlers.InvokePreExecuteHandlers(command);

            command.DbCommand.OpenConnection();

            var dbProviderFactory = DbProviderFactories.GetFactory(command.DbCommand?.Connection!);

            var dataAdapter = dbProviderFactory?.CreateDataAdapter() ??
                throw new Exception("An unexpected null was returned from a call to DbProviderFactory.CreateDataAdapter().");

            dataAdapter.SelectCommand = command.DbCommand;

            dataAdapter.Fill(dataSet);

            EventHandlers.InvokePostExecuteHandlers(command);
        }
        catch (Exception exception)
        {
            EventHandlers.InvokeUnhandledExceptionHandlers(exception, command);

            throw;
        }
        finally
        {
            if (!keepAlive)
            {
                command.DbCommand.CloseAndDispose();
                command.DbCommand = null!;

            }
        }

        return dataSet;
    }

    public static DataTable? ExecuteToDataTable(this DatabaseCommand command, bool keepAlive = false)
    {
        return command.ExecuteToDataSet(keepAlive).Tables[0];
    }
    #endregion

    #region transactions
    public static DbTransaction? BeginTransaction(this DatabaseCommand command)
    {
        var transaction = command.DbCommand.BeginTransaction();

        return transaction;
    }

    public static DbTransaction? BeginTransaction(this DatabaseCommand command, IsolationLevel isolationLevel)
    {
        var transaction = command.DbCommand.BeginTransaction(isolationLevel);

        return transaction;
    }
    #endregion

    #region miscellaneous
    public static bool TestConnection(this DatabaseCommand command)
    {
        try
        {
            command.DbCommand?.Connection?.Open();
            command.DbCommand?.Connection?.Close();

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static DbCommand? ToDbCommand(this DatabaseCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        return command.DbCommand;
    }

    /// <summary>
    /// Generate a DTO with command and connection information.
    /// </summary>
    internal static DebugInfo? GetDebugInfo(this DatabaseCommand command)
    {
        var dbCommand = command.DbCommand ?? throw new Exception("The database command is not initialized.");
        var connection = dbCommand.Connection ?? throw new Exception("The underlying command connection is null.");

        return new DebugInfo
        {
            MachineName = Environment.MachineName,
            HostName = Dns.GetHostEntry("localhost").HostName,
            DataSource = connection.DataSource,
            Database = connection.Database,
            ConnectionString = connection.ConnectionString,
            ConnectionState = connection.State,
            CommandTimeout = dbCommand.CommandTimeout,
            CommandParameterCount = dbCommand.Parameters.Count,
            AnnotatedCommandText = dbCommand.AnnotatedCommandText()
        };
    }
    #endregion
}
