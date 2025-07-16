using System.Data;

namespace Stellar.DAL;

/// <summary>
/// An object with host, connection and command information.
/// </summary>
internal class DatabaseCommandDebugInfo
{
    public string? MachineName;
    public string? HostName;
    public string? DataSource;
    public string? Database;
    public string? ConnectionString;
    public ConnectionState ConnectionState;
    public int CommandTimeout;
    public int CommandParameterCount;
    public string? AnnotatedCommandText;
}
