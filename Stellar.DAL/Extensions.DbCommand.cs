using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;

namespace Stellar.DAL;

/// <summary>
/// <see cref="DbCommand" /> fluent extensions.
/// </summary>
public static partial class Extensions
{
    #region connection
    public static DbCommand OpenConnection(this DbCommand command)
    {
        if (command.Connection.State != ConnectionState.Open)
        {
            command.Connection.Open();
        }

        return command;
    }

    public static void CloseAndDispose(this DbCommand command)
    {
        command.Connection.Close();

        command.Connection.Dispose();

        command.Dispose();
    }
    #endregion

    #region setters
    public static DbCommand SetCommandText(this DbCommand command, string text)
    {
        command.CommandText = text;

        return command;
    }

    public static DbCommand AppendCommandText(this DbCommand command, string text)
    {
        command.CommandText += text;

        return command;
    }

    public static DbCommand SetCommandType(this DbCommand command, CommandType commandType)
    {
        command.CommandType = commandType;

        return command;
    }

    public static DbCommand SetCommandTimeout(this DbCommand command, int commandTimeoutSeconds)
    {
        command.CommandTimeout = commandTimeoutSeconds;

        return command;
    }

    public static DbCommand SetTransaction(this DbCommand command, DbTransaction dbTransaction)
    {
        command.Transaction = dbTransaction;

        return command;
    }
    #endregion

    #region parameter helpers
    public static DbParameter CreateParameter(this DbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();

        parameter.ParameterName = name;
        parameter.Value = value;

        return parameter;
    }

    public static DbParameter CreateParameter(this DbCommand command, string name, object value, DbType type)
    {
        var parameter = command.CreateParameter();

        parameter.ParameterName = name;
        parameter.Value = value;
        parameter.DbType = type;

        return parameter;
    }

    public static DbParameter CreateParameter(this DbCommand command, string name, object value, DbType type, ParameterDirection direction)
    {
        var parameter = command.CreateParameter();

        parameter.ParameterName = name;
        parameter.Value = value;
        parameter.DbType = type;
        parameter.Direction = direction;

        return parameter;
    }

    public static DbCommand AddParameter(this DbCommand command, DbParameter parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter);

        command.Parameters.Add(parameter);

        return command;
    }

    public static DbCommand AddParameter(this DbCommand command, string name, object value)
    {
        
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var parameter = command.CreateParameter(name, value);

        command.Parameters.Add(parameter);

        return command;
    }

    public static DbCommand AddParameter(this DbCommand command, string name, object value, DbType type)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var parameter = command.CreateParameter(name, value, type);

        command.Parameters.Add(parameter);

        return command;
    }

    public static DbCommand AddParameters(this DbCommand command, IEnumerable<DbParameter> parameters)
    {
        foreach (var parameter in parameters)
        {
            command.AddParameter(parameter);
        }

        return command;
    }

    public static DbCommand AddParameters(this DbCommand command, params DbParameter[] parameters)
    {
        foreach (var parameter in parameters)
        {
            command.AddParameter(parameter);
        }

        return command;
    }

    public static DbCommand AddParameters(this DbCommand command, IDictionary<string, object> parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        foreach (var parameter in parameters)
        {
            command.AddParameter(parameter.Key, parameter.Value);
        }

        return command;
    }

    /// <summary>Replaces the name parameter in <see cref="DbCommand.CommandText" /> with a comma-delimited
    /// list of ordered parameter names, i.e., expands 'IN(@names)' into 'IN (@names_p0,@names_p1,@names_p2)'.</summary>
    public static DbCommand AddParameters<T>(this DbCommand command, string name, List<T> parameterValues)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(command.CommandText);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(parameterValues);
        
        if (parameterValues.Count == 0)
        {
            throw new Exception("Parameter values list is empty.");
        }

        if (command.CommandText.Contains(name) == false)
        {
            throw new Exception($"The CommandText does not contain the parameter name '{name}'");
        }

        var parameterNames = new List<string>();

        foreach (var value in parameterValues)
        {
            var paramName = $"{name}_p{command.Parameters.Count}";

            parameterNames.Add(paramName);

            command.AddParameter(paramName, value);
        }

        var commaDelimitedString = string.Join(',', parameterNames);

        command.CommandText = command.CommandText.Replace(name, commaDelimitedString);

        return command;
    }

    public static DbCommand AddParameters<T>(this DbCommand command, string name, List<T> parameterValues, DbType type)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(command.CommandText);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(parameterValues);

        if (parameterValues.Count == 0)
        {
            throw new Exception("Parameter values list is empty.");
        }

        if (command.CommandText.Contains(name) == false)
        {
            throw new Exception($"The CommandText does not contain the parameter name '{name}'");
        }

        var parameterNames = new List<string>();

        foreach (var value in parameterValues)
        {
            var paramName = $"{name}_p{command.Parameters.Count}";

            parameterNames.Add(paramName);

            command.AddParameter(paramName, value, type);
        }

        var commaDelimitedString = string.Join(',', parameterNames);

        command.CommandText = command.CommandText.Replace(name, commaDelimitedString);

        return command;
    }
    #endregion

    #region insert script generators
    private static readonly string AnsiSqlInsert = @$"INSERT INTO {{0}}({{1}}) VALUES({{2}});";

    public static DbCommand GenerateInsertCommand(this DbCommand command, object item, string template, string table = null, KeywordEscapeMethod keywordEscapeMethod = KeywordEscapeMethod.None/*, string output = null*/)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentException.ThrowIfNullOrWhiteSpace(template);

        if (!(template.Contains("{0}") && template.Contains("{1}") && template.Contains("{2}")))
        {
            throw new Exception($"The template must define three arguments, e.g., {AnsiSqlInsert}.");
        }

        if (string.IsNullOrWhiteSpace(table) && item.IsAnonymousType())
        {
            throw new ArgumentNullException(nameof(table), "table is required for anonymous type items.");
        }

        var p = string.Empty;
        var s = string.Empty;

        switch (keywordEscapeMethod)
        {
            case KeywordEscapeMethod.SquareBracket: p = "[";  s = "]";  break;
            case KeywordEscapeMethod.DoubleQuote:   p = "\""; s = "\""; break;
            case KeywordEscapeMethod.Backtick:      p = "`";  s = "`";  break;
            case KeywordEscapeMethod.None:
                goto default;
            default:
                break;
        }

        table ??= $"{p}{item.GetType().Name}{s}";

        var columns = new List<string>();
        var values = new List<string>();

        var namesAndValues = TypeCache.GetMetadataAndValues(item);

        foreach (var (key, value) in namesAndValues)
        {
            if (value == null)
            {
                continue;
            }

            var name = $"@{key}_p{command.Parameters.Count}";

            columns.Add($"{p}{key}{s}");
            values.Add(name);

            command.AddParameter(name, value);
        }

        command.AppendCommandText(string.Format(template,
            table,
            string.Join(',', columns),
            string.Join(',', values)));

        return command;
    }

    #region MySql
    public static string MySqlInsert { get; set; } = AnsiSqlInsert + "SELECT LAST_INSERT_ID();";

    public static DbCommand GenerateMySqlInsert(this DbCommand command, object item, string table = null)
    {
        return command.GenerateInsertCommand(item, MySqlInsert, table, KeywordEscapeMethod.Backtick);
    }

    public static DbCommand GenerateMySqlInserts<T>(this DbCommand command, List<T> list, string table = null)
    {
        foreach (var item in list)
        {
            command.GenerateInsertCommand(item, MySqlInsert, table, KeywordEscapeMethod.Backtick);
        }

        return command;
    }
    #endregion

    #region SQLite
    public static string SQLiteInsert { get; set; } = AnsiSqlInsert + "SELECT last_insert_rowid();";

    public static DbCommand GenerateSQLiteInsert(this DbCommand command, object item, string table = null)
    {
        return command.GenerateInsertCommand(item, SQLiteInsert, table, KeywordEscapeMethod.SquareBracket);
    }

    public static DbCommand GenerateSQLiteInserts<T>(this DbCommand command, List<T> list, string table = null)
    {
        foreach (var item in list)
        {
            command.GenerateInsertCommand(item, SQLiteInsert, table, KeywordEscapeMethod.SquareBracket);
        }

        return command;
    }
    #endregion

    #region SqlServer
    private static readonly string SqlServerInsert = AnsiSqlInsert + "SELECT SCOPE_IDENTITY();";

    private static readonly string SqlServerInsertWithOutput = @$"INSERT INTO {{0}}({{1}}) OUTPUT Inserted.* VALUES({{2}});{Environment.NewLine}";

    public static DbCommand GenerateSqlServerInsert(this DbCommand command, object item, string table = null)
    {
        return command.GenerateInsertCommand(item, SqlServerInsert, table, KeywordEscapeMethod.SquareBracket);
    }

    public static DbCommand GenerateSqlServerInsertWithOutput(this DbCommand command, object item, string table = null)
    {
        return command.GenerateInsertCommand(item, SqlServerInsertWithOutput, table, KeywordEscapeMethod.SquareBracket);
    }

    public static DbCommand GenerateSqlServerInserts<T>(this DbCommand command, List<T> list, string table = null)
    {
        foreach (var item in list)
        {
            command.GenerateInsertCommand(item, SqlServerInsertWithOutput, table, KeywordEscapeMethod.SquareBracket);
        }
        
        return command;
    }

    public static DbCommand GenerateAggregateInsertForSqlServer(this DbCommand command, object item, string table = null)
    {
        return command.GenerateInsertCommand(item, SqlServerInsertWithOutput, table, KeywordEscapeMethod.SquareBracket);
    }
    #endregion

    #endregion

    #region select script generators
    private static readonly string SqlServerSelectTemplate = @$"SELECT {{1}} FROM {{0}} WHERE ({{2}});";

    private static readonly string SqlServerSelectByIdTemplate = @$"SELECT * FROM {{0}} WHERE ({{1}}Id = @{{1}}Id);";

    public static DbCommand GenerateSqlServerSelectById(this DbCommand command, string table, long id)
    {
        return command.GenerateSelectByIdCommand(SqlServerSelectByIdTemplate, table, id, KeywordEscapeMethod.SquareBracket);
    }

    public static DbCommand GenerateSelectByIdCommand(this DbCommand command, string template, string table, long objId, KeywordEscapeMethod keywordEscapeMethod = KeywordEscapeMethod.None)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(template);

        if (template.Contains("{0}") == false || template.Contains("{1}") == false)
        {
            throw new Exception("The template does not conform to the requirements of containing two arguments.");
        }

        var prefix = string.Empty;
        var suffix = string.Empty;

        switch (keywordEscapeMethod)
        {
            case KeywordEscapeMethod.SquareBracket:
                prefix = "[";
                suffix = "]";
                break;
            case KeywordEscapeMethod.DoubleQuote:
                prefix = "\"";
                suffix = "\"";
                break;
            case KeywordEscapeMethod.Backtick:
                prefix = "`";
                suffix = "`";
                break;
            case KeywordEscapeMethod.None:
                goto default;
            default:
                break;
        }

        command.AppendCommandText(string.Format(template, $"{prefix}{table}{suffix}", table))
            .AddParameter($"@{table}Id", objId);

        return command;
    }
    #endregion

    #region transactions; TODO: commit and rollback?
    public static DbTransaction BeginTransaction(this DbCommand command)
    {
        command.OpenConnection();

        var transaction = command.Connection.BeginTransaction();

        command.SetTransaction(transaction);

        return transaction;
    }

    public static DbTransaction BeginTransaction(this DbCommand command, IsolationLevel isolationLevel)
    {
        command.OpenConnection();

        var transaction = command.Connection.BeginTransaction(isolationLevel);

        command.SetTransaction(transaction);

        return transaction;
    }
    #endregion

    #region Miscellaneous
    /// <summary>Returns a new instance of a <see cref="DatabaseCommand" />.</summary>
    /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
    /// <returns><see cref="DatabaseCommand" /> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="dbCommand" /> parameter is null.</exception>
    public static DatabaseCommand ToDatabaseCommand(this DbCommand dbCommand)
    {
        ArgumentNullException.ThrowIfNull(dbCommand);

        return new DatabaseCommand(dbCommand);
    }

    /// <summary>Gets the command text annotated with parameter names and values.</summary>
    /// <param name="command"><see cref="DbCommand" /> instance.</param>
    /// <returns>Annotated command text.</returns>
    internal static string AnnotatedCommandText(this DbCommand command)
    {
        return command.Parameters
            .Cast<DbParameter>()
            .Aggregate(
                command.CommandText,
                (current, parameter) => Regex.Replace(current, parameter.ParameterName, $"/*{parameter.ParameterName}=*/{parameter.Value}"));
    }
    #endregion
}
