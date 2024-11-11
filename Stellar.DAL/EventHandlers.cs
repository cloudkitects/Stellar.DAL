using System;
using System.Collections.Generic;

namespace Stellar.DAL;

/// <summary>
/// Pre-execute, post-execute and unhandled exception <see cref="DatabaseCommand" /> event handlers.
/// </summary>
public static class EventHandlers
{
    #region delegates
    public delegate void PreExecute(DatabaseCommand databaseCommand);

    public delegate void PostExecute(DatabaseCommand databaseCommand);

    public delegate void UnhandledException(Exception exception, DatabaseCommand databaseCommand);
    #endregion

    #region delegate collections
    public static readonly List<PreExecute> PreExecuteHandlers = [];

    public static readonly List<PostExecute> PostExecuteHandlers = [];
    
    public static readonly List<UnhandledException> UnhandledExceptionHandlers = [];
    #endregion

    #region invocation
    public static void InvokePreExecuteHandlers(DatabaseCommand databaseCommand)
    {
        foreach (var handler in PreExecuteHandlers)
        {
            handler.Invoke(databaseCommand);
        }
    }

    public static void InvokePostExecuteHandlers(DatabaseCommand databaseCommand)
    {
        foreach (var handler in PostExecuteHandlers)
        {
            handler.Invoke(databaseCommand);
        }
    }

    public static void InvokeUnhandledExceptionHandlers(Exception exception, DatabaseCommand databaseCommand)
    {
        foreach (var handler in UnhandledExceptionHandlers)
        {
            handler.Invoke(exception, databaseCommand);
        }
    }
    #endregion
}
