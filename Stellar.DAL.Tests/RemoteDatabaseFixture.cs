using System.Collections.Concurrent;

using Azure.Core;
using Azure.Identity;
using Microsoft.Data.SqlClient;

namespace Stellar.DAL.Tests;

public class RemoteDatabaseFixture : IDisposable
{
    const string defaultScopeSuffix = "/.default";

    private readonly string _connectionString = "Server=tcp:stellardev.database.windows.net;Database=AdventureWorks;User Id=5d17220e-009d-4608-ac3d-238706831a5f;Encrypt=True;";

    // take advantage of underlying token caches
    private static readonly ConcurrentDictionary<string, DefaultAzureCredential> credentials = new();

    void IDisposable.Dispose()
    {
        GC.SuppressFinalize(this);
    }

    #region helpers
    // a shared callback ensures connections are created in the same pool
    private static readonly Func<SqlAuthenticationParameters, CancellationToken, Task<SqlAuthenticationToken>> GetTokenCallback =
        async (authParams, cancellationToken) =>
        {
            //auth params are derived from the connection string
            var scope = authParams.Resource.EndsWith(defaultScopeSuffix)
                ? authParams.Resource
                : $"{authParams.Resource}{defaultScopeSuffix}";

            var options = new DefaultAzureCredentialOptions
            {
                ExcludeEnvironmentCredential = true,
                TenantId = "7e0542af-25b3-485a-b072-e010f52803b3",
                ManagedIdentityClientId = authParams.UserId
            };

            // cached credentials will exist the second time around for the same user managed identity
            var token = await credentials.GetOrAdd(authParams.UserId, new DefaultAzureCredential(options)).GetTokenAsync(
                new TokenRequestContext([ scope ]),
                cancellationToken);

            return new SqlAuthenticationToken(token.Token, token.ExpiresOn);
        };


    /// <summary>
    /// Gets a <see cref="DbCommand"/> for testing purposes.
    /// </summary>
    /// <returns><see cref="DbCommand"/> instance.</returns>
    public DatabaseCommand GetCommand()
    {
        return new DatabaseClient<SqlConnection>(_connectionString, GetTokenCallback)
            .GetCommand();
    }
    #endregion
}