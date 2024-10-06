using System;
using System.Collections.Generic;

namespace Stellar.DAL;

/// <summary>
/// Pre-execute, post-execute and unhandled exception <see cref="DatabaseCommand" /> event handlers.
/// </summary>
public static class EventHandlers
{
    #region delegates
    public delegate void PreExecuteEventHandler(DatabaseCommand databaseCommand);

    public delegate void PostExecuteEventHandler(DatabaseCommand databaseCommand);

    public delegate void UnhandledExceptionEventHandler(Exception exception, DatabaseCommand databaseCommand);
    #endregion

    #region delegate collections
    public static readonly List<PreExecuteEventHandler> PreExecuteEventHandlers = [];

    public static readonly List<PostExecuteEventHandler> PostExecuteEventHandlers = [];
    
    public static readonly List<UnhandledExceptionEventHandler> UnhandledExceptionEventHandlers = [];
    #endregion

    #region invocation
    public static void InvokePreExecuteEventHandlers(DatabaseCommand databaseCommand)
    {
        foreach (var handler in PreExecuteEventHandlers)
        {
            handler.Invoke(databaseCommand);
        }
    }

    public static void InvokePostExecuteEventHandlers(DatabaseCommand databaseCommand)
    {
        foreach (var handler in PostExecuteEventHandlers)
        {
            handler.Invoke(databaseCommand);
        }
    }

    public static void InvokeUnhandledExceptionEventHandlers(Exception exception, DatabaseCommand databaseCommand)
    {
        foreach (var handler in UnhandledExceptionEventHandlers)
        {
            handler.Invoke(exception, databaseCommand);
        }
    }
    #endregion
}
