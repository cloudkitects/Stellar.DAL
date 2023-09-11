using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("Stellar.DAL.Tests")]
namespace Stellar.DAL
{
    /// <summary>
    /// Wrapper class for this library.
    /// </summary>
    public static class DatabaseClient
    {
        #region Create Connection
        /// <summary>Creates a <see cref="DbConnection" />.</summary>
        /// <param name="connectionString">Connection string.</param>
        /// <param name="accessToken">Optional access token.</param>
        /// <returns>A new <see cref="DbConnection" /> instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="connectionString" /> parameter is null.</exception>
        /// <exception cref="Exception">An unknown error occurred creating a connection</exception>
        public static DbConnection CreateConnection(string connectionString, string accessToken = null)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            var connection = new SqlConnection
            {
                AccessToken = accessToken,
                ConnectionString = connectionString
            } ?? throw new Exception("Unable to create a System.Data.SqlClient.SqlConnection with the provided values.");
            
            connection.ConnectionString = connectionString;

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
        /// <param name="connectionString"></param>
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
        public static DatabaseCommand GetCommand(string connectionString = null)
        {
            return new(CreateConnection(connectionString));
        }
        #endregion
    }
}
