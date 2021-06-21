using System;

namespace Stellar.DAL.Model
{
    /// <summary>
    /// Instructs the DAL framework about the target schema and table for commands
    /// that do not specify a table name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class EntityAttribute : Attribute
    {
        public string Table { get; }

        public string Schema { get; }

        public EntityAttribute(string schema, string table = null)
        {
            Schema = schema;
            Table = table;
        }
    }
}