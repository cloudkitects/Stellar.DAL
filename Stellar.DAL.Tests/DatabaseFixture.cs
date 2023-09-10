﻿namespace Stellar.DAL.Tests;

public class DatabaseFixture : IDisposable
{
    /// <summary>
    /// A connection string to a MS SQL Server local instance.
    /// </summary>
    private readonly string _connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;Integrated Security=True;";

    public DatabaseFixture()
    {
        var database = TestHelpers.TestDbName();

        var sql1 = TestHelpers.ParseSqlFile(@"Data\DropDatabase.sql", database); ExecuteSql(sql1);
        var sql2 = TestHelpers.ParseSqlFile(@"Data\CreateDatabase.sql", database); ExecuteSql(sql2);
        var sql3 = TestHelpers.ParseSqlFile(@"Data\CreateTables.sql", database); ExecuteSql(sql3);

        _connectionString += $"Initial Catalog={database};";

        GetCommand()
            .SetCommandText(@$"BULK INSERT Person FROM '{Directory.GetCurrentDirectory()}\Data\persons2.tsv';")
            .ExecuteNonQuery();
            
        GetCommand()
            .SetCommandText(@$"BULK INSERT [Address] FROM '{Directory.GetCurrentDirectory()}\Data\Addresses.tsv';")
            .ExecuteNonQuery();
    }

    void IDisposable.Dispose()
    {
        //var sql = TestHelpers.ParseSqlFile(@"Data\DropDatabase.sql", _database); ExecuteSql(sql);

        //GC.SuppressFinalize(this);
    }

    #region helpers
    public long ExecuteSql(string sql)
    {
        return GetCommand()
            .SetCommandText(sql)
            .ExecuteNonQuery();
    }

    /// <summary>
    /// Gets a <see cref="DbCommand"/> for testing purposes.
    /// </summary>
    /// <returns><see cref="DbCommand"/> instance.</returns>
    public DatabaseCommand GetCommand()
    {
        return DatabaseClient.GetCommand(_connectionString);
    }

    /// <summary>
    /// Get a SQL Server INSERT statement based on the object type.
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="table"></param>
    /// <returns>A database command.</returns>
    public DatabaseCommand GetInsertCommand(object instance, string? table = null)
    {
        return GetCommand()
            .GenerateInsertForSqlServer(instance, table);
    }

    /// <summary>
    /// Get a SQL Server INSERT statement and execute it.
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="table"></param>
    /// <returns>The number of rows affected.</returns>
    public long Save(object instance, string? table = null)
    {
        return GetInsertCommand(instance, table)
            .ExecuteNonQuery();
    }

    /// <summary>
    /// Get a SQL Server INSERT statement, execute it and return the saved entity Id.
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="table"></param>
    /// <returns>The saved entity Id.</returns>
    public long SavedId(object instance, string? table = null)
    {
        return GetInsertCommand(instance, table)
            .ExecuteScalar<long>();
    }

    /// <summary>
    /// Get a SQL Server INSERT statement and execute it to the database identity value.
    /// </summary>
    /// <param name="table"></param>
    /// <param name="id"></param>
    /// <returns>The number of rows affected.</returns>
    public DatabaseCommand GetSelectByIdCommand(string table, long id)
    {
        return GetCommand()
            .GenerateSelectByIdForSqlServer(table, id);
    }
    #endregion
}