using System.Data.Common;
using System.Runtime.CompilerServices;

using Microsoft.Data.SqlClient;

[assembly: InternalsVisibleTo("Stellar.DAL.Tests")]
namespace Stellar.DAL;

/// <summary>
/// Wrapper class for this library.
/// </summary>
public class DatabaseClient<T>(string connectionString, Func<SqlAuthenticationParameters, CancellationToken, Task<SqlAuthenticationToken>>? accessTokenCallback = null) where T : DbConnection, new()
{
    #region Create Connection
    /// <summary>Creates a <see cref="DbConnection" of the specified subtype />.</summary>
    public T CreateConnection()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        T connection;

        if (typeof(T) == typeof(SqlConnection) && accessTokenCallback is not null)
        {
            connection = new SqlConnection()
            {
                ConnectionString = connectionString,
                AccessTokenCallback = accessTokenCallback
            } as T ?? throw new ArgumentException("Unable to create a SQLConnection instance.");
        }
        else
        {
            connection = new T()
            {
                ConnectionString = connectionString
            };
        }

        return connection;
    }
    #endregion

    #region Get Command
    /// <summary>Gets a <see cref="DatabaseCommand" /> given a <see cref="DbCommand" /> instance.</summary>
    public DatabaseCommand GetCommand(DbCommand dbCommand)
    {
        return new(dbCommand);
    }

    /// <summary>Gets a <see cref="DatabaseCommand" /> given a <see cref="DbConnection" /> instance.</summary>
    public DatabaseCommand GetCommand(DbConnection dbConnection)
    {
        return new(dbConnection);
    }

    /// <summary>Get a <see cref="DatabaseCommand" /> based on creation time parameters.</summary>
    public DatabaseCommand GetCommand()
    {
        return new(CreateConnection());
    }
    #endregion
}