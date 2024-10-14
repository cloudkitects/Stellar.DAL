# Stellar Data Access Layer
The evolution of Randy Burden's [Sequelocity.NET](https://github.com/randyburden/Sequelocity.NET).

Beyond trying to keep up with .NET and data access libraries' upgrades and splitting code into multiple files, this DAL attempts to (1) include Snowflake, (2) enhance auto-mapping and (3) encapsulate an Entity Framework.

## The Database Client

The generic `DatabaseClient<T> where T : DbConnection` takes in a connection string and exposes the APIs for almost every database interaction, yet it **does not** build a connection string or read it from configuration files or the options pattern app settings.

Users are free to use the System's data `DbConnection` object to build a connection string and pass the result to the client, but a simple string (or an interpolated string) works just as well:

```cs
[Fact] 
public void ConnectsToLocalDb() 
{ 
    var connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;Integrated Security=True;Initial Catalog=master";

    var cmd = new DatabaseClient<SqlConnection>(connectionString) 
        .GetCommand() 
        .SetCommandText("SELECT 1;"); 

    Assert.Equal(1, cmd.ExecuteScalar()); 
}
```
## The Azure Database Client

The specialized `AzureDatabaseClient` derives from the generic class, takes in a connection string and wraps the token callback and caching logic.

### User managed identity authentication flow

By default, it takes in a connection string where the user is a managed identity resource, e.g:

```cs
    private readonly string connectionString =
        "Server=tcp:<server_name>.database.windows.net;" +
        "Database=AdventureWorks;" +
        "User Id=<MANAGED_IDENTITY_CLIENT_ID>;" +
        "Encrypt=True;";
```
> [!CAUTION]
> It is strongly recommended to setup system environment variables for credentials. Avoid hardcoding them, never mind pushing them (the git commit history is unforgiving).

The underlying system libraries derive the resource and the tenant Id out of the connection string.

### App registration authetication flow

App registrations are Microsoft Entra (former Active Directory) principals. You'll need afew extra steps on your machine and on the database itself.

For starter, you'll need all the `AZURE_*` Ids from your Azure tenant:

![image](https://github.com/user-attachments/assets/f98484c2-a227-45f6-9f99-7e12e7c46f48)

You must also register the identity on the database itself and give it the right authorization:
```sql
-- create a database from an identity that exists in the directory
CREATE USER [your_app_name] FROM EXTERNAL PROVIDER;

-- add to roles as needed
ALTER ROLE <db_datareader> ADD MEMBER <your_app_name>;
ALTER ROLE <db_datawriter> ADD MEMBER <your_app_name>;

-- grant permissions as needed
GRANT VIEW DEFINITION [ON <object>] TO <your_app_name>;
```
Thanks to all of that (and the underlying default credentials flow) your connection string becomes a lot more secure:

```sql
var connectionString = "Server=<server_name>.database.windows.net,1433;Initial Catalog=<database_name>;Connect Timeout=30"
```

### Azure caveats
the API
The remote database fixture tests are tied to a specific Azure subscription and Azure SQL Server, and therefore will fail from your box. Besides installing the latest `Az.Accounts` PowerShell module, issuing an `az login` and selecting the right subscription, currently a subscription admin or owner has to add a firewall rule, so again, remote tests will fail.

We're considering adding a broad rule, but please consider excluding them with a trait or setting up your own trial Azure subscription and managed identity in the meantime.

Remote tests are also known to time out initially for serverless databases. Consider increasing the timeout in the connection string or the API calls and rerunning them if/when timeout exceptions are thrown.
