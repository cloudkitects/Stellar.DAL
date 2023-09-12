using System;

namespace Stellar.DAL
{
    /// <summary>A Type conversion helper.</summary>
    public static class TypeConverter
    {
        /// <summary>Converts the given value to the given type.</summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="type">Type to convert the given value to.</param>
        /// <returns>Converted value.</returns>
        /// <exception cref="TypeConversionException">An error occurs attempting to convert a value to an enum or type.</exception>
        public static object Convert(object value, Type type)
        {
            if (value == DBNull.Value)
            {
                value = null;
            }

            if (value == null && type.IsValueType)
            {
                return type.GetDefaultValue();
            }

            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

            try
            {
                if (underlyingType.IsEnum)
                {
                    value = Enum.Parse(underlyingType, value.ToString(), true);
                }
                else if (underlyingType == typeof(Guid))
                {
                    value = value switch
                    {
                        string str => new Guid(str),
                        byte[] bytes => new Guid(bytes),
                        _ => value
                    };
                }

                var result = System.Convert.ChangeType(value, underlyingType);

                return result;
            }
            catch (Exception exception)
            {
                throw new TypeConversionException($"Unable to convert {value} to {underlyingType}.", exception);
            }
        }
    }
}
