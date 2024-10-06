# Stellar Data Access Layer
The evolution of Randy Burden's [Sequelocity.NET](https://github.com/randyburden/Sequelocity.NET).

Beyond trying to keep up with .NET and data access libraries' upgrades and splitting code into multiple files, this DAL attempts to (1) include Snowflake, (2) enhance auto-mapping and (3) encapsulate an Entity Framework.

The database-agnostic `DatabaseClient` takes in a connection string and exposes the APIs for almost every database interaction, yet it **does not** build a connection string or read it from configuration files or the options pattern app settings.

Users are free to use the System's data `DbConnection` object to build a connection string and pass the result to the client, but simple string interpolation works as well:

https://github.com/cloudkitects/Stellar.DAL/blob/bf5a8f9a912ddd37e04c8b17689c160753e29ac0/Stellar.DAL.Tests/DbClientTests.cs#L7-19

