using System;
using System.Collections.Generic;

namespace Stellar.DAL;

/// <summary>
/// <see cref="DatabaseCommand" /> event handlers.
/// </summary>
public static class EventHandlers
{
    /// <summary>Event handler called after the <see cref="DatabaseCommand" /> has been executed.</summary>
    /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
    public delegate void DatabaseCommandPostExecuteEventHandler(DatabaseCommand databaseCommand);

    /// <summary>Event handler called before the <see cref="DatabaseCommand" /> is executed.</summary>
    /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
    public delegate void DatabaseCommandPreExecuteEventHandler(DatabaseCommand databaseCommand);

    /// <summary>Event handler called when an unhandled exception occurs.</summary>
    /// <param name="exception">Unhandled exception.</param>
    /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
    public delegate void DatabaseCommandUnhandledExceptionEventHandler(Exception exception, DatabaseCommand databaseCommand);

    /// <summary>Event triggered when an unhandled exception occurs.</summary>
    public static readonly List<DatabaseCommandUnhandledExceptionEventHandler> DatabaseCommandUnhandledExceptionEventHandlers = [];

    /// <summary>Event triggered before the <see cref="DatabaseCommand" /> is executed.</summary>
    public static readonly List<DatabaseCommandPreExecuteEventHandler> DatabaseCommandPreExecuteEventHandlers = [];

    /// <summary>Event triggered after the <see cref="DatabaseCommand" /> has been executed.</summary>
    public static readonly List<DatabaseCommandPostExecuteEventHandler> DatabaseCommandPostExecuteEventHandlers = [];

    /// <summary>Invokes the <see cref="DatabaseCommand" /> unhandled exception event handlers.</summary>
    /// <param name="exception">Unhandled exception.</param>
    /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
    public static void InvokeDatabaseCommandUnhandledExceptionEventHandlers(Exception exception, DatabaseCommand databaseCommand)
    {
        foreach (var databaseCommandUnhandledExceptionEventHandler in DatabaseCommandUnhandledExceptionEventHandlers)
        {
            databaseCommandUnhandledExceptionEventHandler.Invoke(exception, databaseCommand);
        }
    }

    /// <summary>Invokes the <see cref="DatabaseCommand" /> pre-execute event handlers.</summary>
    /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
    public static void InvokeDatabaseCommandPreExecuteEventHandlers(DatabaseCommand databaseCommand)
    {
        foreach (var databaseCommandPreExecuteEventHandler in DatabaseCommandPreExecuteEventHandlers)
        {
            databaseCommandPreExecuteEventHandler.Invoke(databaseCommand);
        }
    }

    /// <summary>Invokes the <see cref="DatabaseCommand" /> post-execute event handlers.</summary>
    /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
    public static void InvokeDatabaseCommandPostExecuteEventHandlers(DatabaseCommand databaseCommand)
    {
        foreach (var databaseCommandPostExecuteEventHandler in DatabaseCommandPostExecuteEventHandlers)
        {
            databaseCommandPostExecuteEventHandler.Invoke(databaseCommand);
        }
    }
}
