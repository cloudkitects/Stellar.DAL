using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Stellar.DAL.Model;

namespace Stellar.DAL
{
    public static partial class Extensions
    {
        #region Insert templates
        /// <summary>
        /// The ANSI SQL INSERT command template. Timeless :)
        /// </summary>
        private static readonly string AnsiSqlInsertTemplate = @$"INSERT INTO {{0}}({{1}}) VALUES({{2}});{Environment.NewLine}";

        /// <summary>
        /// The SQL Server INSERT command template--outputs the inserted object(s) back.
        /// </summary>
        private static readonly string SqlServerInsertTemplate = @$"INSERT INTO {{0}}({{1}}) OUTPUT Inserted.* VALUES({{2}});{Environment.NewLine}";

        /// <summary>
        /// The SQL Server INSERT command template.
        /// </summary>
        ///private static readonly string SqlServerInsertTemplate = AnsiSqlInsertTemplate + "SELECT SCOPE_IDENTITY() AS [LastInsertedId];";

        /// <summary>
        /// MySql INSERT command template, returns the last inserted id(s).
        /// </summary>
        public static string MySqlInsertTemplate { get; set; } = AnsiSqlInsertTemplate + "SELECT LAST_INSERT_ID() AS LastInsertedId;";

        /// <summary>
        /// SqLite INSERT command template, returns the last inserted row id(s).
        /// </summary>
        public static string SqLiteInsertTemplate { get; set; } = AnsiSqlInsertTemplate + "SELECT last_insert_rowid() AS [LastInsertedId];";
        #endregion

        #region Select templates
        /// <summary>
        /// The SQL Server SELECT command template.
        /// </summary>
        ///private static readonly string SqlServerSelectTemplate = @$"SELECT {{1}} FROM {{0}} WHERE ({{2}});{Environment.NewLine}";

        /// <summary>
        /// Custom SQL Server SELECT.
        /// </summary>
        private static readonly string SqlServerSelectByIdTemplate = @$"SELECT * FROM {{0}} WHERE ({{1}}Id = @{{1}}Id);{Environment.NewLine}";
        #endregion

        #region Set properties
        /// <summary>Sets the text command to run against the data source.</summary>
        /// <param name="command"><see cref="DbCommand" /> instance.</param>
        /// <param name="text">The text command to run against the data source.</param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        public static DbCommand SetCommandText(this DbCommand command, string text)
        {
            command.CommandText = text;

            return command;
        }

        /// <summary>Appends to the text command to run against the data source.</summary>
        /// <param name="command"><see cref="DbCommand" /> instance.</param>
        /// <param name="text">Text command to append to the text command to run against the data source.</param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        public static DbCommand AppendCommandText(this DbCommand command, string text)
        {
            command.CommandText += text;

            return command;
        }

        /// <summary>Sets the CommandType.</summary>
        /// <param name="command"><see cref="DbCommand" /> instance.</param>
        /// <param name="commandType">CommandType which specifies how a command string is interpreted.</param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        public static DbCommand SetCommandType(this DbCommand command, CommandType commandType)
        {
            command.CommandType = commandType;

            return command;
        }

        /// <summary>
        /// Sets the time in seconds to wait for the command to execute before throwing an exception. The default is 30 seconds.
        /// </summary>
        /// <param name="command"><see cref="DbCommand" /> instance.</param>
        /// <param name="commandTimeoutSeconds">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        public static DbCommand SetCommandTimeout(this DbCommand command, int commandTimeoutSeconds)
        {
            command.CommandTimeout = commandTimeoutSeconds;

            return command;
        }
        #endregion

        #region Parameters
        /// <summary>Creates a <see cref="DbParameter" />.</summary>
        /// <param name="command"><see cref="DbCommand" /> instance.</param>
        /// <param name="name">Parameter name.</param>
        /// <param name="value">Parameter value.</param>
        /// <returns><see cref="DbParameter" />.</returns>
        public static DbParameter CreateParameter(this DbCommand command, string name, object value)
        {
            var parameter = command.CreateParameter();

            parameter.ParameterName = name;
            parameter.Value = value;

            return parameter;
        }

        /// <summary>Creates a <see cref="DbParameter" /> with a given <see cref="DbType" />.</summary>
        /// <param name="command"><see cref="DbCommand" /> instance.</param>
        /// <param name="name">Parameter name.</param>
        /// <param name="value">Parameter value.</param>
        /// <param name="type">Parameter type.</param>
        /// <returns><see cref="DbParameter" />.</returns>
        public static DbParameter CreateParameter(this DbCommand command, string name, object value, DbType type)
        {
            var parameter = command.CreateParameter();

            parameter.ParameterName = name;
            parameter.Value = value;
            parameter.DbType = type;

            return parameter;
        }

        /// <summary>
        /// Creates a <see cref="DbParameter" /> with a given <see cref="DbType" /> and <see cref="ParameterDirection" />.
        /// </summary>
        /// <param name="command"><see cref="DbCommand" /> instance.</param>
        /// <param name="name">Parameter name.</param>
        /// <param name="value">Parameter value.</param>
        /// <param name="type">Parameter type.</param>
        /// <param name="direction">Parameter direction.</param>
        /// <returns><see cref="DbParameter" />.</returns>
        public static DbParameter CreateParameter(this DbCommand command, string name, object value, DbType type, ParameterDirection direction)
        {
            var parameter = command.CreateParameter();

            parameter.ParameterName = name;
            parameter.Value = value;
            parameter.DbType = type;
            parameter.Direction = direction;

            return parameter;
        }

        /// <summary>Adds a <see cref="DbParameter" /> to the <see cref="DbCommand" />.</summary>
        /// <param name="command"><see cref="DbCommand" /> instance.</param>
        /// <param name="dbParameter"><see cref="DbParameter" /> to add.</param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="dbParameter" /> parameter is null.</exception>
        public static DbCommand AddParameter(this DbCommand command, DbParameter dbParameter)
        {
            if (dbParameter == null)
            {
                throw new ArgumentNullException(nameof(dbParameter));
            }

            command.Parameters.Add(dbParameter);

            return command;
        }

        /// <summary>Adds a parameter to the <see cref="DbCommand" />.</summary>
        /// <param name="command"><see cref="DbCommand" /> instance.</param>
        /// <param name="name">Parameter name.</param>
        /// <param name="value">Parameter value.</param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="name" /> parameter is null.</exception>
        public static DbCommand AddParameter(this DbCommand command, string name, object value)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var parameter = command.CreateParameter(name, value);

            command.Parameters.Add(parameter);

            return command;
        }

        /// <summary>Adds a parameter to the <see cref="DbCommand" />.</summary>
        /// <param name="command"><see cref="DbCommand" /> instance.</param>
        /// <param name="name">Parameter name.</param>
        /// <param name="value">Parameter value.</param>
        /// <param name="type">Parameter type.</param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="name" /> parameter is null.</exception>
        public static DbCommand AddParameter(this DbCommand command, string name, object value, DbType type)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var parameter = command.CreateParameter(name, value, type);

            command.Parameters.Add(parameter);

            return command;
        }

        /// <summary>Adds a list of <see cref="DbParameter" />s to the <see cref="DbCommand" />.</summary>
        /// <param name="command"><see cref="DbCommand" /> instance.</param>
        /// <param name="dbParameters">List of database parameters.</param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        public static DbCommand AddParameters(this DbCommand command, IEnumerable<DbParameter> dbParameters)
        {
            foreach (var dbParameter in dbParameters)
            {
                command.AddParameter(dbParameter);
            }

            return command;
        }

        /// <summary>Adds a parameter array of <see cref="DbParameter" />s to the <see cref="DbCommand" />.</summary>
        /// <param name="command"><see cref="DbCommand" /> instance.</param>
        /// <param name="dbParameters">Parameter array of database parameters.</param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        public static DbCommand AddParameters(this DbCommand command, params DbParameter[] dbParameters)
        {
            foreach (var dbParameter in dbParameters)
            {
                command.AddParameter(dbParameter);
            }

            return command;
        }

        /// <summary>Adds a dictionary of parameter names and values to the <see cref="DbCommand" />.</summary>
        /// <param name="command"><see cref="DbCommand" /> instance.</param>
        /// <param name="parameterNameAndValueDictionary">Dictionary of parameter names and values.</param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the <paramref name="parameterNameAndValueDictionary" /> parameter
        /// is null.
        /// </exception>
        public static DbCommand AddParameters(this DbCommand command, IDictionary<string, object> parameterNameAndValueDictionary)
        {
            if (parameterNameAndValueDictionary == null)
            {
                throw new ArgumentNullException(nameof(parameterNameAndValueDictionary));
            }

            foreach (var parameterNameAndValue in parameterNameAndValueDictionary)
            {
                command.AddParameter(parameterNameAndValue.Key, parameterNameAndValue.Value);
            }

            return command;
        }

        /// <summary>
        /// Adds the list of parameter values to the <see cref="DbCommand" /> by replacing the given name in the
        /// <see cref="DbCommand.CommandText" /> with a comma delimited list of generated parameter names such as "parameterName0,
        /// parameterName1, parameterName2", etc.
        /// </summary>
        /// <typeparam name="T">Parameter type.</typeparam>
        /// <param name="command"><see cref="DbCommand" /> instance.</param>
        /// <param name="name">Parameter name.</param>
        /// <param name="parameterValues">Parameter values.</param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="name" /> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="parameterValues" /> parameter is null.</exception>
        /// <exception cref="Exception">Thrown when the <paramref name="parameterValues" /> list is empty.</exception>
        /// <exception cref="Exception">
        /// Thrown when the <paramref name="command" /> CommandText has not been set prior to calling this method.
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown when the <paramref name="command" /> CommandText does not contain the
        /// <paramref name="name" />.
        /// </exception>
        public static DbCommand AddParameters<T>(this DbCommand command, string name, List<T> parameterValues)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (parameterValues == null)
            {
                throw new ArgumentNullException(nameof(parameterValues));
            }

            if (parameterValues.Count == 0)
            {
                throw new Exception("Parameter values list is empty.");
            }

            if (string.IsNullOrWhiteSpace(command.CommandText))
            {
                throw new Exception("The CommandText must already be set before calling this method.");
            }

            if (command.CommandText.Contains(name) == false)
            {
                throw new Exception($"The CommandText does not contain the parameter name '{name}'");
            }

            var parameterNames = new List<string>();

            foreach (var value in parameterValues)
            {
                // Note that we are appending the ordinal parameter position as a suffix to the parameter name in order to create
                // some uniqueness for each parameter name as well as to aid in debugging.
                var paramName = name + "_p" + command.Parameters.Count;

                parameterNames.Add(paramName);

                command.AddParameter(paramName, value);
            }

            var commaDelimitedString = string.Join(",", parameterNames);

            command.CommandText = command.CommandText.Replace(name, commaDelimitedString);

            return command;
        }

        /// <summary>
        /// Adds the list of parameter values of the specified <see cref="DbType" /> to the <see cref="DbCommand" /> by replacing
        /// the given name in the <see cref="DbCommand.CommandText" /> with a comma delimited list of generated parameter
        /// names such as "parameterName0, parameterName1, parameterName2", etc.
        /// </summary>
        /// <typeparam name="T">Parameter type.</typeparam>
        /// <param name="command"><see cref="DbCommand" /> instance.</param>
        /// <param name="name">Parameter name.</param>
        /// <param name="parameterValues">Parameter values.</param>
        /// <param name="type">Parameter type.</param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="name" /> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="parameterValues" /> parameter is null.</exception>
        /// <exception cref="Exception">Thrown when the <paramref name="parameterValues" /> list is empty.</exception>
        /// <exception cref="Exception">
        /// Thrown when the <paramref name="command" /> CommandText has not been set prior to calling this method.
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown when the <paramref name="command" /> CommandText does not contain the
        /// <paramref name="name" />.
        /// </exception>
        public static DbCommand AddParameters<T>(this DbCommand command, string name, List<T> parameterValues, DbType type)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (parameterValues == null)
            {
                throw new ArgumentNullException(nameof(parameterValues));
            }

            if (parameterValues.Count == 0)
            {
                throw new Exception("Parameter values list is empty.");
            }

            if (string.IsNullOrWhiteSpace(command.CommandText))
            {
                throw new Exception("The CommandText must already be set before calling this method.");
            }

            if (command.CommandText.Contains(name) == false)
            {
                throw new Exception($"The CommandText does not contain the parameter name '{name}'");
            }

            var parameterNames = new List<string>();

            foreach (var value in parameterValues)
            {
                // Note that we are appending the ordinal parameter position as a suffix to the parameter name in order to create
                // some uniqueness for each parameter name as well as to aid in debugging.
                var paramName = name + "_p" + command.Parameters.Count;

                parameterNames.Add(paramName);

                command.AddParameter(paramName, value, type);
            }

            var commaDelimitedString = string.Join(",", parameterNames);

            command.CommandText = command.CommandText.Replace(name, commaDelimitedString);

            return command;
        }
        #endregion

        #region Generate inserts
        #region MySql
        /// <summary>
        /// Generates a parameterized MySQL INSERT statement from the given object and adds it to the <see cref="DbCommand" />.
        /// <para>
        /// The generated query selects the last inserted id, i.e. it relies on DBMS built-in row identity feature and a round-trip.
        /// </para>
        /// </summary>
        /// <param name="command"><see cref="DbCommand" /> instance.</param>
        /// <param name="item">Object to generate the SQL INSERT statement from.</param>
        /// <param name="table">
        /// Optional table name to insert into. If none is supplied, it will use the type name. Note that this parameter is
        /// required when passing in an anonymous object or an <see cref="ArgumentNullException" /> will be thrown.
        /// </param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// The value of 'table' cannot be null when the object passed is an anonymous
        /// type.
        /// </exception>
        public static DbCommand GenerateInsertForMySql(this DbCommand command, object item, string table = null)
        {
            return command.GenerateInsertCommand(item, MySqlInsertTemplate, table, KeywordEscapeMethod.Backtick);
        }

        /// <summary>
        /// Generates a list of concatenated parameterized MySQL INSERT statements from the given list of objects and adds it to
        /// the <see cref="DbCommand" />.
        /// <para>
        /// The generated query selects the last inserted id, i.e. it relies on DBMS built-in row identity feature and a round-trip.
        /// </para>
        /// </summary>
        /// <typeparam name="T">Type of the objects in the list.</typeparam>
        /// <param name="command"><see cref="DbCommand" /> instance.</param>
        /// <param name="list">List of objects to generate the SQL INSERT statements from.</param>
        /// <param name="table">
        /// Optional table name to insert into. If none is supplied, it will use the type name. Note that this parameter is
        /// required when passing in an anonymous object or an <see cref="ArgumentNullException" /> will be thrown.
        /// </param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// The value of 'table' cannot be null when the object passed is an anonymous
        /// type.
        /// </exception>
        public static DbCommand GenerateInsertsForMySql<T>(this DbCommand command, List<T> list, string table = null)
        {
            foreach (var item in list)
            {
                command.GenerateInsertCommand(item, MySqlInsertTemplate, table, KeywordEscapeMethod.Backtick);
            }

            return command;
        }
        #endregion

        #region SqLite
        /// <summary>
        /// Generates a parameterized SQLite INSERT statement from the given object and adds it to the <see cref="DbCommand" />
        /// .
        /// <para>
        /// The generated query selects the last inserted id, i.e. it relies on DBMS built-in row identity feature and a round-trip.
        /// </para>
        /// </summary>
        /// <param name="command"><see cref="DbCommand" /> instance.</param>
        /// <param name="item">Object to generate the SQL INSERT statement from.</param>
        /// <param name="table">
        /// Optional table name to insert into. If none is supplied, it will use the type name. Note that this parameter is
        /// required when passing in an anonymous object or an <see cref="ArgumentNullException" /> will be thrown.
        /// </param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// The value of 'table' cannot be null when the object passed is an anonymous
        /// type.
        /// </exception>
        // ReSharper disable once InconsistentNaming
        public static DbCommand GenerateInsertForSQLite(this DbCommand command, object item, string table = null)
        {
            return command.GenerateInsertCommand(item, SqLiteInsertTemplate, table, KeywordEscapeMethod.SquareBracket);
        }

        /// <summary>
        /// Generates a list of concatenated parameterized SQLite INSERT statements from the given list of objects and adds it to
        /// the <see cref="DbCommand" />.
        /// <para>
        /// The generated query selects the last inserted id, i.e. it relies on DBMS built-in row identity feature and a round-trip.
        /// </para>
        /// </summary>
        /// <typeparam name="T">Type of the objects in the list.</typeparam>
        /// <param name="command"><see cref="DbCommand" /> instance.</param>
        /// <param name="list">List of objects to generate the SQL INSERT statements from.</param>
        /// <param name="table">
        /// Optional table name to insert into. If none is supplied, it will use the type name. Note that this parameter is
        /// required when passing in an anonymous object or an <see cref="ArgumentNullException" /> will be thrown.
        /// </param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// The value of 'table' cannot be null when the object passed is an anonymous
        /// type.
        /// </exception>
        // ReSharper disable once InconsistentNaming
        public static DbCommand GenerateInsertsForSQLite<T>(this DbCommand command, List<T> list, string table = null)
        {
            foreach (var item in list)
            {
                command.GenerateInsertCommand(item, SqLiteInsertTemplate, table, KeywordEscapeMethod.SquareBracket);
            }

            return command;
        }
        #endregion

        #region SqlServer
        /// <summary>
        /// Generates a parameterized SQL Server INSERT statement from the given object and adds it to the
        /// <see cref="DbCommand" />.
        /// <para>
        /// Note that the generated query also selects the last inserted id using SQL Server's SELECT SCOPE_IDENTITY() function.
        /// </para>
        /// </summary>
        /// <param name="command"><see cref="DbCommand" /> instance.</param>
        /// <param name="item">Object to generate the SQL INSERT statement from.</param>
        /// <param name="table">
        /// Optional table name to insert into. If none is supplied, it will use the type name. Note that this parameter is
        /// required when passing in an anonymous object or an <see cref="ArgumentNullException" /> will be thrown.
        /// </param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// The value of 'table' cannot be null when the object passed is an anonymous type.
        /// </exception>
        public static DbCommand GenerateInsertForSqlServer(this DbCommand command, object item, string table = null)
        {
            return command.GenerateInsertCommand(item, SqlServerInsertTemplate, table, KeywordEscapeMethod.SquareBracket);
        }

        /// <summary>
        /// Generates a list of concatenated parameterized SQL Server INSERT statements from the given list of objects and adds it
        /// to the <see cref="DbCommand" />.
        /// <para>
        /// Note that the generated query also selects the last inserted id using SQL Server's SELECT SCOPE_IDENTITY() function.
        /// </para>
        /// </summary>
        /// <typeparam name="T">Type of the objects in the list.</typeparam>
        /// <param name="command"><see cref="DbCommand" /> instance.</param>
        /// <param name="list">List of objects to generate the SQL INSERT statements from.</param>
        /// <param name="table">
        /// Optional table name to insert into. If none is supplied, it will use the type name. Note that this parameter is
        /// required when passing in an anonymous object or an <see cref="ArgumentNullException" /> will be thrown.
        /// </param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// The value of 'table' cannot be null when the object passed is an anonymous
        /// type.
        /// </exception>
        public static DbCommand GenerateInsertsForSqlServer<T>(this DbCommand command, List<T> list, string table = null)
        {
            foreach (var item in list)
            {
                command.GenerateInsertCommand(item, SqlServerInsertTemplate, table, KeywordEscapeMethod.SquareBracket);
            }
            
            return command;
        }

        public static DbCommand GenerateAggregateInsertForSqlServer(this DbCommand command, object item, string table = null)
        {
            return command.GenerateInsertCommand(item, SqlServerInsertTemplate, table, KeywordEscapeMethod.SquareBracket);
        }
        #endregion

        /// <summary>
        /// Generates a parameterized SQL Server INSERT statement from the given object and adds it to the
        /// <see cref="DbCommand" />.
        /// </summary>
        /// <param name="command"><see cref="DbCommand" /> instance.</param>
        /// <param name="item">Object to generate the SQL INSERT statement from.</param>
        /// <param name="template">
        /// SQL INSERT statement template where argument 0 is the table name, argument 1 is the comma delimited list of columns,
        /// and argument 2 is the comma delimited list of values.
        /// <para>Example: INSERT INTO {0} ({1}) VALUES({2});</para>
        /// </param>
        /// <param name="table">
        /// Optional table name to insert into. If none is supplied, it will use the type name. Note that this parameter is
        /// required when passing in an anonymous object or an <see cref="ArgumentNullException" /> will be thrown.
        /// </param>
        /// <param name="keywordEscapeMethod">The method used for escaping keywords.</param>
        /// <param name="output"></param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// The value of 'table' cannot be null when the object passed is an anonymous
        /// type.
        /// </exception>
        /// <remarks>
        /// Notes: the (template == null) check is encapsulated by the next (string.IsNullOrWhiteSpace(template)) check without
        /// much overhead (the first check of the equality comparer). See <a href="https://stackoverflow.com/questions/18507715">Why is string.IsNullOprEmpty faster than comparison</a>
        /// for more.
        /// </remarks>
        public static DbCommand GenerateInsertCommand(this DbCommand command, object item, string template, string table = null, KeywordEscapeMethod keywordEscapeMethod = KeywordEscapeMethod.None, string output = null)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (string.IsNullOrWhiteSpace(template))
            {
                throw new ArgumentNullException(nameof(template), "The parameter must not be null, empty, or whitespace.");
            }

            if (template.Contains("{0}") == false || template.Contains("{1}") == false || template.Contains("{2}") == false)
            {
                throw new Exception("The template does not conform to the requirements of containing three arguments, e.g. 'INSERT INTO {0} ({1}) VALUES({2});'");
            }

            if (table == null && item.IsAnonymousType())
            {
                throw new ArgumentNullException(nameof(table), "The 'table' parameter must be provided when the object supplied is an anonymous type.");
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

            // get schema & table from object attribute
            table ??= BuildEntityName(item, prefix, suffix);

            var columns = new StringBuilder();
            var values = new StringBuilder();

            var namesAndValues = TypeCache.GetMetadataAndValues(item);

            foreach (var (key, value) in namesAndValues)
            {
                if (value == null)
                {
                    continue;
                }

                var name = $"@{key}_p{command.Parameters.Count}";

                columns.Append($"{prefix}{key}{suffix},");
                values.Append($"{name},");

                command.AddParameter(name, value);
            }

            command.AppendCommandText(string.Format(template,
                table,
                columns.ToString().TrimEnd(','),
                values.ToString().TrimEnd(',')));
            

            return command;
        }
        #endregion

        #region Generate select

        /// <summary>
        /// Generates a parameterized SQL Server SELECT statement from the given object and adds it to the
        /// <see cref="DbCommand" />.
        /// </summary>
        /// <param name="command"><see cref="DbCommand" /> instance.</param>
        /// <param name="table">
        /// Optional table name to insert into. If none is supplied, it will use the type name. Note that this parameter is
        /// required when passing in an anonymous or dynamic object or an <see cref="ArgumentNullException" /> will be thrown.
        /// </param>
        /// <param name="id">The id to get.</param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// The value of 'table' cannot be null when the object passed is an anonymous or dynamic.
        /// </exception>
        public static DbCommand GenerateSelectByIdForSqlServer(this DbCommand command, string table, long id)
        {
            return command.GenerateSelectByIdCommand(SqlServerSelectByIdTemplate, table, id, KeywordEscapeMethod.SquareBracket);
        }

        /// <summary>
        /// Generates a parameterized SQL Server SELECT statement from the given object and adds it to the
        /// <see cref="DbCommand" />.
        /// </summary>
        /// <param name="command"><see cref="DbCommand" /> instance.</param>
        /// <param name="template">
        /// SQL INSERT statement template where argument 0 is the table name, argument 1 is the comma delimited list of columns,
        /// and argument 2 is the WHERE clause.
        /// <para>Example: SELECT ({0}) FROM {0}Id WHERE ({0}Id = @{0}Id);</para>
        /// </param>
        /// <param name="table">
        /// Optional table name to insert into. If none is supplied, it will use the type name. Note that this parameter is
        /// required when passing in an anonymous object or an <see cref="ArgumentNullException" /> will be thrown.
        /// </param>
        /// <param name="objId">The object Id to get, added as a parameter.</param>
        /// <param name="keywordEscapeMethod">The method used for escaping keywords.</param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// The value of 'table' cannot be null when the object passed is an anonymous
        /// type.
        /// </exception>
        /// <remarks>
        /// Notes: the (template == null) check is encapsulated by the next (string.IsNullOrWhiteSpace(template)) check without
        /// much overhead (the first check of the equality comparer). See <a href="https://stackoverflow.com/questions/18507715">Why is string.IsNullOprEmpty faster than comparison</a>
        /// for more.
        /// </remarks>
        public static DbCommand GenerateSelectByIdCommand(this DbCommand command, string template, string table, long objId, KeywordEscapeMethod keywordEscapeMethod = KeywordEscapeMethod.None)
        {
            if (string.IsNullOrWhiteSpace(template))
            {
                throw new ArgumentNullException(nameof(template), "The parameter must not be null, empty, or whitespace.");
            }

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

        #region Transactions
        /// <summary>Sets the transaction associated with the command.</summary>
        /// <param name="command"><see cref="DbCommand" /> instance.</param>
        /// <param name="dbTransaction">The transaction to associate with the command.</param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        public static DbCommand SetTransaction(this DbCommand command, DbTransaction dbTransaction)
        {
            command.Transaction = dbTransaction;

            return command;
        }

        /// <summary>
        /// Starts a database transaction and associates it with the <see cref="DbCommand"/> instance.
        /// </summary>
        /// <param name="command"><see cref="DbCommand" /> instance.</param>
        /// <returns>An object representing the new transaction.</returns>
        public static DbTransaction BeginTransaction(this DbCommand command)
        {
            command.OpenConnection();

            var transaction = command.Connection.BeginTransaction();

            command.SetTransaction(transaction);

            return transaction;
        }

        /// <summary>
        /// Starts a database transaction with the specified isolation level and associates it with the <see cref="DbCommand"/> instance.
        /// </summary>
        /// <param name="command"><see cref="DbCommand" /> instance.</param>
        /// <param name="isolationLevel">Specifies the isolation level for the transaction.</param>
        /// <returns>An object representing the new transaction.</returns>
        public static DbTransaction BeginTransaction(this DbCommand command, IsolationLevel isolationLevel)
        {
            command.OpenConnection();

            var transaction = command.Connection.BeginTransaction(isolationLevel);

            command.SetTransaction(transaction);

            return transaction;
        }
        #endregion

        #region Connection
        /// <summary>Opens a database connection.</summary>
        /// <param name="command"><see cref="DbCommand" /> instance.</param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        public static DbCommand OpenConnection(this DbCommand command)
        {
            if (command.Connection.State != ConnectionState.Open)
            {
                command.Connection.Open();
            }

            return command;
        }

        /// <summary>
        /// Closes and disposes the <see cref="DbCommand.Connection" /> and the <see cref="DbCommand" /> itself.
        /// </summary>
        /// <param name="command"><see cref="DbCommand" /> instance.</param>
        public static void CloseAndDispose(this DbCommand command)
        {
            command.Connection.Close();

            command.Connection.Dispose();

            command.Dispose();
        }
        #endregion

        #region Miscellaneous
        /// <summary>Returns a new instance of a <see cref="DatabaseCommand" />.</summary>
        /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
        /// <returns><see cref="DatabaseCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="dbCommand" /> parameter is null.</exception>
        public static DatabaseCommand ToDatabaseCommand(this DbCommand dbCommand)
        {
            if (dbCommand == null)
            {
                throw new ArgumentNullException(nameof(dbCommand));
            }

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
                    (current, parameter) => Regex.Replace(current, $"{parameter.ParameterName}", $"/*{parameter.ParameterName}=*/'{parameter.Value}'"));
        }

        /// <summary>
        /// Build a conforming entity name from the entity attribute or the type name.
        /// </summary>
        /// <param name="obj">The object to build a data service name for.</param>
        /// <param name="prefix">The data service compliant name prefix.</param>
        /// <param name="suffix">The data service compliant name suffix.</param>
        /// <returns>A one-part or two-part data service complaint name for an entity
        /// as defined byt the attribute or the type.</returns>
        /// <remarks>Best used as a fallback when the entity name is not explicitly defined.
        /// </remarks>
        internal static string BuildEntityName(object obj, string prefix, string suffix)
        {
            var type = obj.GetType();

            var attribute = (EntityAttribute)TypeDescriptor.GetAttributes(obj)[typeof(EntityAttribute)];

            if (attribute == null && (!Attribute.IsDefined(type, typeof(EntityAttribute)) || (attribute = (EntityAttribute)Attribute.GetCustomAttribute(type, typeof(EntityAttribute))) == null))
            {
                return $"{prefix}{type.Name}{suffix}";
            }

            var schema = string.IsNullOrWhiteSpace(attribute.Schema)
                ? string.Empty
                : $"{prefix}{attribute.Schema}{suffix}.";
            
            return $"{schema}{prefix}{attribute.Table ?? type.Name}{suffix}";
        }

        #endregion
    }
}
