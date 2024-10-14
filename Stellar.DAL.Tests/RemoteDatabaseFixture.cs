namespace Stellar.DAL.Tests;

public class RemoteDatabaseFixture : IDisposable
{
    private readonly string _connectionString = 
        "Server=tcp:stellardev.database.windows.net;" +
        "Database=AdventureWorks;" +
        "User Id=5d17220e-009d-4608-ac3d-238706831a5f;" +
        "Encrypt=True;";

    void IDisposable.Dispose()
    {
        GC.SuppressFinalize(this);
    }


    /// <summary>
    /// Gets a <see cref="DbCommand"/> for testing purposes.
    /// </summary>
    /// <returns><see cref="DbCommand"/> instance.</returns>
    public DatabaseCommand GetCommand()
    {
        return new AzureDatabaseClient(_connectionString)
            .GetCommand();
    }
}