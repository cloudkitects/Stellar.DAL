using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

using Azure.Core;
using Azure.Identity;
using Microsoft.Data.SqlClient;

namespace Stellar.DAL;

public class AzureDatabaseClient(string connectionString) : DatabaseClient<SqlConnection>(connectionString, GetTokenCallback)
{
    // cache underlying token caches
    private static readonly ConcurrentDictionary<string, DefaultAzureCredential> credentials = new();

    private static readonly Func<SqlAuthenticationParameters, CancellationToken, Task<SqlAuthenticationToken>> GetTokenCallback =
        async (authParams, cancellationToken) =>
        {
            const string defaultScopeSuffix = "/.default";

            // derive auth params from the connection string
            var scope = authParams.Resource.EndsWith(defaultScopeSuffix)
                ? authParams.Resource
                : $"{authParams.Resource}{defaultScopeSuffix}";

            var tenantId = new Uri(authParams.Authority).PathAndQuery.Trim('/');

            var options = new DefaultAzureCredentialOptions
            {
                ExcludeEnvironmentCredential = true,
                TenantId = tenantId,
                ManagedIdentityClientId = authParams.UserId
            };

            // cached credentials will be reused for the same user managed identity
            var token = await credentials.GetOrAdd(authParams.UserId, new DefaultAzureCredential(options)).GetTokenAsync(
                new TokenRequestContext([scope]),
                cancellationToken);

            return new SqlAuthenticationToken(token.Token, token.ExpiresOn);
        };
}
