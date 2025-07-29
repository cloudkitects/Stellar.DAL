namespace Stellar.DAL;

[Serializable]
public class ConnectionStringNotFoundException(string message = "Unable to find a database connection string. A valid connection string value or name must be supplied or assigned to the 'ConfigurationSettings.Default.ConnectionStringName' or the 'ConfigurationSettings.Default.ConnectionString' property.") : Exception(message)
{
}

[Serializable]
public class TypeConversionException(string message, Exception innerException) : Exception(message, innerException)
{
}
