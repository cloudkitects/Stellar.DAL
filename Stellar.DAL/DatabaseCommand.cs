using System;
using System.Data.Common;

namespace Stellar.DAL;

/// <summary>
/// Lightweight <see cref="DbCommand" /> wrapper that
/// helps provide a fluent data access interface.
/// </summary>
public class DatabaseCommand : IDisposable
{
    #region fields and properties
    /// <summary>Whether Dispose has been called.</summary>
    private bool _disposed;

    /// <summary>The underlying <see cref="DbCommand" />.</summary>
    public DbCommand DbCommand;
    #endregion

    #region contructors
    /// <summary>Instantiates a new <see cref="DatabaseCommand" /> from a <see cref="System.Data.Common.DbCommand" />.</summary>
    /// <param name="dbCommand">DbCommand.</param>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="dbCommand" /> parameter is null.</exception>
    public DatabaseCommand(DbCommand dbCommand)
    {
        DbCommand = dbCommand ?? throw new ArgumentNullException(nameof(dbCommand));
    }

    /// <summary>Instantiates a new <see cref="DatabaseCommand" /> from a <see cref="DbConnection" />.</summary>
    /// <param name="dbConnection">DbConnection.</param>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="dbConnection" /> parameter is null.</exception>
    public DatabaseCommand(DbConnection dbConnection)
    {
        ArgumentNullException.ThrowIfNull(dbConnection);

        DbCommand = dbConnection.CreateCommand();
    }
    #endregion

    #region IDisposable Members
    /// <summary>Disposes of the underlying <see cref="DbCommand" /> and it's <see cref="DbConnection" />.</summary>
    public void Dispose()
    {
        Dispose(true);

        // Use in case a subclass implements a finalizer
        GC.SuppressFinalize(this);
    }

    /// <summary>Disposes of the underlying <see cref="DbCommand" />.</summary>
    /// <param name="disposing">Indicates if being called from the Dispose method.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            if (DbCommand != null)
            {
                try
                {
                    DbCommand.Dispose();
                }
                catch
                {
                    // ignore
                }
                finally
                {
                    DbCommand = null;
                }
            }
        }

        _disposed = true;
    }
    #endregion IDisposable Members
}
