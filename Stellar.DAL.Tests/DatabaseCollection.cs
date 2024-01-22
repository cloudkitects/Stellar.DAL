namespace Stellar.DAL.Tests;

/// <summary>
/// These classes have no code, and are never created. They are
/// a placeholder for [CollectionDefinition] and inherit all 
/// ICollectionFixture<> interfaces.
/// </summary>

[CollectionDefinition("Local Database collection")]
public class LocalDatabaseCollection : ICollectionFixture<LocalDatabaseFixture>
{
}

[CollectionDefinition("Remote Database collection")]
public class RemoteDatabaseCollection : ICollectionFixture<RemoteDatabaseFixture>
{
}