using System;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using Npgsql;
using Snowflake.Data.Client;

[assembly: InternalsVisibleTo("Stellar.DAL.Tests")]
namespace Stellar.DAL;

/// <summary>
/// Wrapper class for this library.
/// </summary>
public class DatabaseClient(string connectionString, Rdbms rdbms = Rdbms.SqlServer, string accessToken = null)
{
    #region Create Connection
    /// <summary>Creates a <see cref="DbConnection" of the specified subtype />.</summary>
    /// <returns>A new <see cref="DbConnection" /> instance.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="connectionString" /> parameter is null.</exception>
    /// <exception cref="Exception">An unknown error occurred creating a SQL connection</exception>
    public DbConnection CreateConnection()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        return rdbms switch
        {
            Rdbms.Odbc => CreateOdbcConnection(),
            Rdbms.SqlServer => CreateSqlConnection(),
            Rdbms.MySql => CreateMySqlConnection(),
            Rdbms.SQLite => CreateSQLiteConnection(),
            Rdbms.Postgres => CreatePostgresConnection(),
            Rdbms.Snowflake => CreateSnowflakeConnection(),

            _ => throw new NotImplementedException()
        };
    }

    /// <summary>Creates a <see cref="SqlConnection" />.</summary>
    /// <returns>A new <see cref="DbConnection" /> instance.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="connectionString" /> parameter is null.</exception>
    /// <exception cref="Exception">An unknown error occurred creating a SQL connection</exception>
    public SqlConnection CreateSqlConnection()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        var connection = new SqlConnection
        {
            AccessToken = accessToken,
            ConnectionString = connectionString
        } ?? throw new Exception($"Unable to create a {typeof(SqlConnection)} with the provided values.");

        // TODO: why?
        connection.ConnectionString = connectionString;

        return connection;
    }

    /// <summary>Creates a <see cref="OdbcConnection" />.</summary>
    /// <returns>A new <see cref="DbConnection" /> instance.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="connectionString" /> parameter is null.</exception>
    /// <exception cref="Exception">An unknown error occurred creating a SQL connection</exception>
    public OdbcConnection CreateOdbcConnection()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        var connection = new OdbcConnection
        {
            ConnectionString = connectionString,

        } ?? throw new Exception($"Unable to create a {typeof(OdbcConnection)} with the provided values.");

        return connection;
    }

    /// <summary>Creates a <see cref="SnowflakeDbConnection" />.</summary>
    /// <returns>A new <see cref="DbConnection" /> instance.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="connectionString" /> parameter is null.</exception>
    /// <exception cref="Exception">An unknown error occurred creating a SQL connection</exception>
    public SnowflakeDbConnection CreateSnowflakeConnection()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        var connection = new SnowflakeDbConnection
        {
            ConnectionString = connectionString,

        } ?? throw new Exception($"Unable to create a {typeof(SnowflakeDbConnection)} with the provided values.");

        return connection;
    }

    /// <summary>Creates a <see cref="NpgsqlConnection" />.</summary>
    /// <returns>A new <see cref="DbConnection" /> instance.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="connectionString" /> parameter is null.</exception>
    /// <exception cref="Exception">An unknown error occurred creating a SQL connection</exception>
    public NpgsqlConnection CreatePostgresConnection()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        var connection = new NpgsqlConnection
        {
            ConnectionString = connectionString,

        } ?? throw new Exception($"Unable to create a {typeof(NpgsqlConnection)} with the provided values.");

        return connection;
    }

    /// <summary>Creates a <see cref="MySqlConnection" />.</summary>
    /// <returns>A new <see cref="DbConnection" /> instance.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="connectionString" /> parameter is null.</exception>
    /// <exception cref="Exception">An unknown error occurred creating a SQL connection</exception>
    public MySqlConnection CreateMySqlConnection()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        var connection = new MySqlConnection
        {
            ConnectionString = connectionString,

        } ?? throw new Exception($"Unable to create a {typeof(MySqlConnection)} with the provided values.");

        return connection;
    }

    /// <summary>Creates a <see cref="SqliteConnection" />.</summary>
    /// <returns>A new <see cref="DbConnection" /> instance.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="connectionString" /> parameter is null.</exception>
    /// <exception cref="Exception">An unknown error occurred creating a SQL connection</exception>
    public SqliteConnection CreateSQLiteConnection()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        var connection = new SqliteConnection
        {
            ConnectionString = connectionString,

        } ?? throw new Exception($"Unable to create a {typeof(SqliteConnection)} with the provided values.");

        return connection;
    }
    #endregion

    #region Get Command
    /// <summary>Gets a <see cref="DatabaseCommand" /> given a <see cref="DbCommand" /> instance.</summary>
    /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
    /// <returns>A new <see cref="DatabaseCommand" /> instance.</returns>
    public static DatabaseCommand GetCommand(DbCommand dbCommand)
    {
        return new(dbCommand);
    }

    /// <summary>Gets a <see cref="DatabaseCommand" /> given a <see cref="DbConnection" /> instance.</summary>
    /// <param name="dbConnection"><see cref="DbConnection" /> instance.</param>
    /// <returns>A new <see cref="DatabaseCommand" /> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="dbConnection" /> parameter is null.</exception>
    public static DatabaseCommand GetCommand(DbConnection dbConnection)
    {
        return new(dbConnection);
    }

    /// <summary>Attempts to get a <see cref="DatabaseCommand" /> using several strategies.</summary>
    /// <returns>A new <see cref="DatabaseCommand" /> instance.</returns>
    /// <exception cref="Exception">
    /// Thrown when no ConnectionString could be found. A valid ConnectionString or Connection String Name must be supplied in
    /// the 'connectionString' parameter or by setting a default in either the
    /// 'DatabaseCommand.ConfigurationSettings.Default.ConnectionStringName' or
    /// 'DatabaseCommand.ConfigurationSettings.Default.ConnectionString' properties.
    /// </exception>
    /// <exception cref="DatabaseCommand.DbCommand">
    /// Thrown when no DbProviderFactory could be found. A DbProviderFactory invariant name must be supplied in the connection
    /// string settings 'providerName' attribute in the applications config file, in the 'dbProviderFactoryInvariantName'
    /// parameter, or by setting a default in the
    /// 'DatabaseCommand.ConfigurationSettings.Default.DbProviderFactoryInvariantName' property.
    /// </exception>
    /// <exception cref="DatabaseCommand">
    /// An unknown error occurred creating a connection as the call to DbProviderFactory.CreateConnection() returned null.
    /// </exception>
    public DatabaseCommand GetCommand()
    {
        return new(CreateConnection());
    }
    #endregion
}