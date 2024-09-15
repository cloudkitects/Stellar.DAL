using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Stellar.DAL.Tests")]
namespace Stellar.DAL;

/// <summary>
/// Wrapper class for this library.
/// </summary>
public class DbClient<T>(string connectionString, string accessToken = null) where T : DbConnection, new()
{
    #region Create Connection
    /// <summary>Creates a <see cref="DbConnection" of the specified T subtype />.</summary>
    public T CreateConnection()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        T connection = new T()
        {
            ConnectionString = connectionString
        } ?? throw new Exception($"Unable to create a {typeof(SqlConnection)} with the provided values.");

        if (typeof(T) == typeof(SqlConnection) && accessToken is not null)
        {
            (connection as SqlConnection).AccessToken = accessToken;
        }
        
        return connection;
    }
    #endregion

    #region Get Command
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