using Microsoft.Identity.Client;

using System.Data.SqlClient;

namespace Stellar.DAL.Tests;

public class RemoteDatabaseFixture : IDisposable
{
    private readonly string _connectionString = @"Data Source=azddedpdb1.database.windows.net;Initial Catalog=edp-atlas;Connect Timeout=30;Persist Security Info=False;TrustServerCertificate=False;Encrypt=True;MultipleActiveResultSets=False;";

    private readonly IConfidentialClientApplication client = ConfidentialClientApplicationBuilder
        .Create(Environment.GetEnvironmentVariable("AZURE_CLIENT_ID"))
        .WithClientSecret(Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET"))
        .WithTenantId(Environment.GetEnvironmentVariable("AZURE_TENANT_ID"))
        .Build();

    void IDisposable.Dispose()
    {
        GC.SuppressFinalize(this);
    }

    #region helpers
    public async Task<string> GetToken()
    {
        var result = await client.AcquireTokenForClient(scopes: new[] { "https://database.windows.net/.default" })
            .ExecuteAsync()
            ?? throw new InvalidOperationException("Unable to acquire a database token.");
        
        return result.AccessToken;
    }

    /// <summary>
    /// Gets a <see cref="DbCommand"/> for testing purposes.
    /// </summary>
    /// <returns><see cref="DbCommand"/> instance.</returns>
    public DatabaseCommand GetCommand()
    {
        return new DbClient<SqlConnection>(_connectionString, accessToken: GetToken().Result)
            .GetCommand();
    }
    #endregion
}