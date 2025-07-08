namespace Stellar.DAL;

/// <summary>A Type conversion helper.</summary>
public static class TypeConverter
{
    /// <summary>Converts the given value to the given type.</summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="type">Type to convert the given value to.</param>
    /// <returns>Converted value.</returns>
    /// <exception cref="TypeConversionException">An error occurs attempting to convert a value to an enum.</exception>
    /// <exception cref="TypeConversionException">An error occurs attempting to convert a value to a type.</exception>
    public static object Convert(object? value, Type type)
    {
        // handle DBNull
        if (value == DBNull.Value)
        {
            value = null;
        }

        // handle value type conversion of null to the value type's default value
        if (value == null && type.IsValueType)
        {
            return type.GetDefaultValue()!;
        }

        // handle nullable type
        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

        // handle enums
        if (underlyingType.IsEnum)
        {
            try
            {
                // Because an enum and a nullable enum are both value types, it's actually not possible to reach
                // the next line of code when value is null
                value = Enum.Parse(underlyingType, value?.ToString() ?? string.Empty, true);
            }
            catch (Exception exception)
            {
                throw new TypeConversionException(
                    $"An error occurred while attempting to convert {value} to an enum of type {underlyingType}.", exception);
            }
        }

        try
        {
            // handle GUIDs
            if (underlyingType == typeof(Guid))
            {
                value = value switch
                {
                    string str => new Guid(str),
                    byte[] bytes => new Guid(bytes),
                    _ => value!
                };
            }

            var result = System.Convert.ChangeType(value, underlyingType);

            return result!;
        }
        catch (Exception exception)
        {
            throw new TypeConversionException(
                $"An error occurred while attempting to convert {value} to {underlyingType}.", exception);
        }
    }
}
