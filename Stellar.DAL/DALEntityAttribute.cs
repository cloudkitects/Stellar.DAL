using System;

namespace Stellar.DAL
{
    /// <summary>
    /// Instructs the DAL framework about the target schema and table for commands
    /// that do not specify a table name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DALEntityAttribute : Attribute
    {
        public string Table { get; }

        public string Schema { get; }

        public DALEntityAttribute(string schema, string table = null)
        {
            Schema = schema;
            Table = table;
        }
    }
}