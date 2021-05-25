using System;

namespace Stellar.DAL
{
    /// <summary>Thrown when a ConnectionString could not be found.</summary>
    [Serializable]
    public class ConnectionStringNotFoundException : Exception
    {
        /// <summary>
        /// Instantiates a new <see cref="ConnectionStringNotFoundException" /> with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ConnectionStringNotFoundException(string message = "Unable to find a database connection string. A valid connection string value or name must be supplied or assigned to the 'ConfigurationSettings.Default.ConnectionStringName' or the 'ConfigurationSettings.Default.ConnectionString' property.")
            : base(message)
        {
        }
    }

    /// <summary>Thrown when an exception occurs while converting a value from one type to another.</summary>
    [Serializable]
    public class TypeConversionException : Exception
    {
        /// <summary>Instantiates a new <see cref="TypeConversionException" /> with a specified error message.</summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner
        /// exception is specified.
        /// </param>
        public TypeConversionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
