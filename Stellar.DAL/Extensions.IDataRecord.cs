using System;
using System.Data;
using System.Reflection;

namespace Stellar.DAL
{
    public static partial class Extensions
    {
        /// <summary>Maps an <see cref="IDataRecord" /> to a type of <typeparamref name="T" />.</summary>
        /// <remarks>This method internally uses caching to increase performance.</remarks>
        /// <typeparam name="T">The type to map to.</typeparam>
        /// <param name="dataRecord">The <see cref="IDataRecord" /> to map from.</param>
        /// <returns>A mapped instance of <typeparamref name="T" />.</returns>
        /// <exception cref="TypeConversionException">A value cannot be converted.</exception>
        /// <exception cref="PropertySetValueException">A converted value cannot be assigned to a property.</exception>
        /// <exception cref="FieldSetValueException">A converted value cannot be assigned to a field.</exception>
        public static T ToObject<T>(this IDataRecord dataRecord)
        {
            var fieldCount = dataRecord.FieldCount;
            var type = typeof(T);

            // Handle mapping to primitives and strings when there is only a single field in the record
            if (fieldCount == 1 && (type.IsPrimitive || type == typeof(string)))
            {
                return (T)TypeConverter.Convert(dataRecord.GetValue(0), type);
            }

            var obj = type.GetDefaultValue() ?? Activator.CreateInstance<T>();
            var mapped = false;

            // { case-insensitive property/field name, property/field info }
            var orderedDictionary = TypeCache.GetMetadata(type);

            for (var i = 0; i < fieldCount; i++)
            {
                var fieldName = dataRecord.GetName(i).ToLower();

                // TODO: here's where we'd take advantage of name conversion...
                var memberInfo = orderedDictionary[fieldName];

                switch (memberInfo)
                {
                    case null:
                    case PropertyInfo {CanWrite: false}:
                        continue;
                    case PropertyInfo propertyInfo:
                    {
                        var value = dataRecord.GetValue(i);

                        var convertedValue = TypeConverter.Convert(value, propertyInfo.PropertyType);

                        try
                        {
                            propertyInfo.SetValue(obj, convertedValue, null);

                            mapped = true;
                        }
                        catch (Exception exception)
                        {
                            throw new PropertySetValueException(
                                $"Unable to assign '{convertedValue}' to {type}.{propertyInfo.Name} ({propertyInfo.PropertyType}).", exception);
                        }

                        break;
                    }
                    case FieldInfo fieldInfo:
                    {
                        var value = dataRecord.GetValue(i);

                        var convertedValue = TypeConverter.Convert(value, fieldInfo.FieldType);

                        try
                        {
                            fieldInfo.SetValue(obj, convertedValue);

                            mapped = true;
                        }
                        catch (Exception exception)
                        {
                            throw new FieldSetValueException(
                                $"Error assigning '{value}' to '{type}.{fieldInfo.Name} ({fieldInfo.FieldType})'.", exception);
                        }

                        break;
                    }
                }
            }

            return mapped || fieldCount != 1
                ? (T)obj
                : (T)TypeConverter.Convert(dataRecord.GetValue(0), type);
        }

        /// <summary>Maps an <see cref="IDataRecord" /> to a type of dynamic object.</summary>
        /// <param name="dataRecord">The <see cref="IDataRecord" /> to map from.</param>
        /// <returns>A dynamic object.</returns>
        public static dynamic ToDynamic(this IDataRecord dataRecord)
        {
            dynamic obj = new DynamicDictionary();

            for (var i = 0; i < dataRecord.FieldCount; i++)
            {
                var value = dataRecord.GetValue(i);

                obj[dataRecord.GetName(i)] = value == DBNull.Value ? null : value;
            }

            return obj;
        }

        /// <summary>Exception thrown when setting a fields value.</summary>
        /// <remarks>Instantiates a new <see cref="FieldSetValueException" /> with a specified error message.</remarks>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a null reference if no inner
        /// exception is specified.
        /// </param>
        [Serializable]
        public class FieldSetValueException(string message, Exception innerException) : Exception(message, innerException)
        {
        }

        /// <summary>Exception thrown when setting a properties value.</summary>
        /// <remarks>Instantiates a new <see cref="PropertySetValueException" /> with a specified error message.</remarks>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a null reference if no inner
        /// exception is specified.
        /// </param>
        [Serializable]
        public class PropertySetValueException(string message, Exception innerException) : Exception(message, innerException)
        {
        }
    }
}
