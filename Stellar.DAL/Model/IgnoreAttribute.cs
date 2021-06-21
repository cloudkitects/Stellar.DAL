using System;

namespace Stellar.DAL.Model
{
    /// <summary>
    /// Instructs the DAL framework to ignore the public field or public read/write property value when generating SQL commands.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class IgnoreAttribute : Attribute
    {
    }
}