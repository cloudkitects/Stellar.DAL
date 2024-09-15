namespace Stellar.EF.Model;

/// <summary>
/// Instructs the DAL framework about the target schema and table for commands
/// that do not specify a table name.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class EntityAttribute(string schema, string table = null) : Attribute
{
    public string Table { get; } = table;

    public string Schema { get; } = schema;
}