﻿using Microsoft.Data.SqlClient;

namespace Stellar.DAL.Tests;

public class LocalSqlDatabaseFixture : IDisposable
{
    private readonly string _connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;Integrated Security=True;";

    private static readonly string _database = TestHelpers.TestDbName();

    public LocalSqlDatabaseFixture()
    {
        var sql1 = TestHelpers.ParseSqlFile(@"Data\DropDatabase.sql", _database); ExecuteNonQuery(sql1);
        var sql2 = TestHelpers.ParseSqlFile(@"Data\CreateDatabase.sql", _database); ExecuteNonQuery(sql2);
        var sql3 = TestHelpers.ParseSqlFile(@"Data\CreateTables.sql", _database); ExecuteNonQuery(sql3);

        _connectionString += $"Initial Catalog={_database};";

        GetCommand()
            .SetCommandText(@$"BULK INSERT Person FROM '{Directory.GetCurrentDirectory()}\Data\persons2.tsv';")
            .ExecuteNonQuery();
            
        GetCommand()
            .SetCommandText(@$"BULK INSERT [Address] FROM '{Directory.GetCurrentDirectory()}\Data\Addresses.tsv';")
            .ExecuteNonQuery();
    }

    public void Dispose()
    {
        var sql = TestHelpers.ParseSqlFile(@"Data\DropDatabase.sql", _database);

        ExecuteNonQuery(sql);
    
        GC.SuppressFinalize(this);
    }

    #region helpers
    /// <summary>
    /// Gets a <see cref="DbCommand"/> for testing purposes.
    /// </summary>
    /// <returns><see cref="DbCommand"/> instance.</returns>
    public DatabaseCommand GetCommand()
    {
        return new DatabaseClient<SqlConnection>(_connectionString).GetCommand();
    }

    public long ExecuteNonQuery(string sql)
    {
        return GetCommand()
            .SetCommandText(sql)
            .ExecuteNonQuery();
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
            .GenerateSqlServerInsertWithOutput(instance, table);
    }

    /// <summary>
    /// Get a SQL Server INSERT statement and execute it.
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="table"></param>
    /// <returns>The number of rows affected.</returns>
    public long Insert(object instance, string? table = null)
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
    public long SavedId(object? instance, string? table = null)
    {
        if (instance is null)
        {
            return 0;
        }
        
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
            .GenerateSqlServerSelectById(table, id);
    }
    #endregion
}