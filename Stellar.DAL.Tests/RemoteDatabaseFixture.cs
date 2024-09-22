using Azure.Core;
using Azure.Identity;

using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System.Collections.Concurrent;

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
    public static async Task<string> GetToken()
    {

        //var result = await client.AcquireTokenForClient(scopes: [ "https://database.windows.net/.default" ])
        //    .ExecuteAsync()
        //    ?? throw new InvalidOperationException("Unable to acquire a database token.");

        //return result.AccessToken;

        var tokenCredential = new DefaultAzureCredential();

        var result = await tokenCredential.GetTokenAsync(
            new TokenRequestContext(scopes: ["https://database.windows.net/.default"]) { });

        return result.Token;
    }

    // a shared callback ensures connections are created in the same pool
    private static readonly Func<SqlAuthenticationParameters, CancellationToken, Task<SqlAuthenticationToken>> GetTokenCallback =
        async (authParams, cancellationToken) =>
        {
            var scope = authParams.Resource.EndsWith(defaultScopeSuffix)
                ? authParams.Resource
                : $"{authParams.Resource}{defaultScopeSuffix}";

            var options = new DefaultAzureCredentialOptions
            {
                TenantId = "7e0542af-25b3-485a-b072-e010f52803b3",
                ManagedIdentityClientId = authParams.UserId,
                ExcludeEnvironmentCredential = true
            };

            // Reuse credentials if we are using the same MI Client Id
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