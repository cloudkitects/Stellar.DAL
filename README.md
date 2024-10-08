# Stellar Data Access Layer
The evolution of Randy Burden's [Sequelocity.NET](https://github.com/randyburden/Sequelocity.NET).

Beyond trying to keep up with .NET and data access libraries' upgrades and splitting code into multiple files, this DAL attempts to (1) include Snowflake, (2) enhance auto-mapping and (3) encapsulate an Entity Framework.

The database-agnostic `DatabaseClient` takes in a connection string and exposes the APIs for almost every database interaction, yet it **does not** build a connection string or read it from configuration files or the options pattern app settings.

Users are free to use the System's data `DbConnection` object to build a connection string and pass the result to the client, but a simple string (or an interpolated string) works just as well:

https://github.com/cloudkitects/Stellar.DAL/blob/ab40883de12ebef61e26c8da0a4b6afe66d21890/Stellar.DAL.Tests/DbClientTests.cs#L7-L19

## Testing

The remote database fixture tests are tied to specific Azure subscription and Azure SQL Server and will fail. Besides installing the latest `Az.Accounts` PowerShell module, issuing an `az login` and selecting the right subscription, currently a subscription admin or owner has to add a firewall rule. We're considering adding a broad rule or creating a private endpoint, but please consider excluding them with a trait or using (or setting up) your own Azure subscription and tests in the meantime.
